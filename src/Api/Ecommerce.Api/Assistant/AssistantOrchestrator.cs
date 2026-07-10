using System.Globalization;
using Ecommerce.Api.Assistant.TextToSql;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant;

public sealed class AssistantOrchestrator(
    ISender sender,
    IOrdersAssistantSubAgent ordersAssistantSubAgent,
    IAssistantIntentInterpreter intentInterpreter,
    DeterministicAssistantIntentInterpreter deterministicIntentInterpreter,
    AssistantIntentPlanValidator intentPlanValidator,
    IAssistantTextToSqlPlanner textToSqlPlanner,
    AssistantSqlValidator textToSqlValidator,
    IAssistantReadOnlySqlExecutor textToSqlExecutor,
    AssistantTextToSqlResponseMapper textToSqlResponseMapper,
    ILogger<AssistantOrchestrator> logger,
    IOptions<AssistantLlmOptions> llmOptions,
    IOptions<AssistantTextToSqlOptions> textToSqlOptions)
{
    private const int AnalysisPageNumber = 1;
    private const int AnalysisPageSize = 100;

    public async Task<AssistantQueryResponse> QueryAsync(
        string question,
        Guid buyerId,
        CancellationToken cancellationToken)
    {
        // Text-to-SQL is an optional first-pass path; the existing CQRS assistant flow remains the fallback.
        if (textToSqlOptions.Value.IsEnabled)
        {
            var textToSqlResponse = await TryQueryTextToSqlAsync(question, buyerId, cancellationToken);
            if (textToSqlResponse is not null)
            {
                return textToSqlResponse;
            }
        }

        var intent = await InterpretIntentAsync(question, cancellationToken);

        return intent.Kind switch
        {
            AssistantIntentKind.RecentOrders
                or AssistantIntentKind.TotalSpend
                or AssistantIntentKind.ProductsOrdered
                or AssistantIntentKind.OrdersContainingProduct
                or AssistantIntentKind.OrdersAboveAmount
                or AssistantIntentKind.OrdersContainingProductsOverAmount
                or AssistantIntentKind.ProductFrequency
                => await ordersAssistantSubAgent.HandleAsync(intent, buyerId, cancellationToken),
            AssistantIntentKind.CatalogProductsUnderPrice => await CatalogProductsUnderPriceAsync(intent.Amount ?? 0, cancellationToken),
            AssistantIntentKind.CatalogGetProduct => await CatalogGetProductAsync(intent.ProductId, cancellationToken),
            _ => Unsupported()
        };
    }

    private async Task<AssistantQueryResponse?> TryQueryTextToSqlAsync(
        string question,
        Guid buyerId,
        CancellationToken cancellationToken)
    {
        try
        {
            var plan = await textToSqlPlanner.PlanAsync(question, cancellationToken);
            if (!plan.Supported
                || plan.DataSource is null
                || string.IsNullOrWhiteSpace(plan.Sql))
            {
                LogTextToSqlFallback("planner");
                return null;
            }

            var query = new AssistantSqlQuery(
                plan.DataSource.Value,
                plan.Sql,
                plan.DataSource == AssistantSqlDataSource.Orders ? buyerId : null);
            var validation = textToSqlValidator.Validate(query);
            if (!validation.IsValid)
            {
                LogTextToSqlFallback("validation");
                return null;
            }

            var result = await textToSqlExecutor.ExecuteAsync(query, cancellationToken);
            if (!result.Succeeded)
            {
                LogTextToSqlFallback("execution");
                return null;
            }

            var response = textToSqlResponseMapper.Map(plan, result);
            if (response is null)
            {
                LogTextToSqlFallback("mapping");
            }

            return response;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            LogTextToSqlFallback("exception");
            return null;
        }
    }

    private void LogTextToSqlFallback(string stage)
    {
        logger.LogInformation(
            "Assistant Text-to-SQL orchestration fell back to existing assistant flow at stage={Stage}.",
            stage);
    }

    private async Task<AssistantIntent> InterpretIntentAsync(
        string question,
        CancellationToken cancellationToken)
    {
        LogLlmConfigurationDiagnostics();

        var plan = await TryInterpretAsync(intentInterpreter, question, cancellationToken);
        var deterministicFallbackUsed = false;

        if (plan is null && !ReferenceEquals(intentInterpreter, deterministicIntentInterpreter))
        {
            deterministicFallbackUsed = true;
            plan = await TryInterpretAsync(deterministicIntentInterpreter, question, cancellationToken);
        }

        var validationResult = intentPlanValidator.ValidateWithDiagnostics(question, plan);

        logger.LogInformation(
            "Assistant intent diagnostics: deterministicFallbackUsed={DeterministicFallbackUsed}, modelOutputFailedValidation={ModelOutputFailedValidation}.",
            deterministicFallbackUsed,
            validationResult.ModelOutputFailedValidation);

        return validationResult.Intent;
    }

    private void LogLlmConfigurationDiagnostics()
    {
        var options = llmOptions.Value;

        logger.LogInformation(
            "Assistant LLM configuration diagnostics: llmEnabled={LlmEnabled}, providerEndpointPresent={ProviderEndpointPresent}, providerEndpointValid={ProviderEndpointValid}, modelPresent={ModelPresent}, apiKeyEnvironmentVariableNamePresent={ApiKeyEnvironmentVariableNamePresent}, apiKeyResolved={ApiKeyResolved}.",
            options.Enabled,
            !string.IsNullOrWhiteSpace(options.ResolvedEndpoint),
            IsHttpsEndpoint(options.ResolvedEndpoint),
            !string.IsNullOrWhiteSpace(options.ResolvedModel),
            !string.IsNullOrWhiteSpace(options.ResolvedApiKeyEnvironmentVariable),
            IsApiKeyResolved(options));

        if (!options.Enabled)
        {
            logger.LogInformation(
                "Assistant LLM provider diagnostics: providerCallAttempted={ProviderCallAttempted}, providerCallFailed={ProviderCallFailed}.",
                false,
                false);
        }
    }

    private static async Task<AssistantIntentPlan?> TryInterpretAsync(
        IAssistantIntentInterpreter interpreter,
        string question,
        CancellationToken cancellationToken)
    {
        try
        {
            return await interpreter.InterpretAsync(question, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsApiKeyResolved(AssistantLlmOptions options)
    {
        return options.TryResolveApiKey(out _);
    }

    private static bool IsHttpsEndpoint(string endpoint) =>
        Uri.TryCreate(endpoint, UriKind.Absolute, out var uri)
        && uri.Scheme == Uri.UriSchemeHttps;

    public AssistantQueryResponse Unsupported() =>
        new(
            "I can help with read-only product lookup and your own order history, but I cannot perform that request.",
            Array.Empty<string>(),
            "none",
            true);

    private async Task<AssistantQueryResponse> CatalogProductsUnderPriceAsync(
        decimal amount,
        CancellationToken cancellationToken)
    {
        var products = await sender.Send(
            new SearchProductsQuery(null, true, AnalysisPageNumber, AnalysisPageSize),
            cancellationToken);
        var matches = products.Items
            .Where(product => product.Price < amount)
            .Take(10)
            .ToArray();
        var matchLines = matches
            .Select(product => $"{product.Name} ({product.Sku}) {Money(product.Price)}")
            .ToArray();

        if (matchLines.Length == 0)
        {
            return new AssistantQueryResponse(
                $"I did not find active products under {Money(amount)} in the returned catalog page.",
                [AssistantToolNames.CatalogSearch],
                "catalog-public",
                false);
        }

        return new AssistantQueryResponse(
            $"Products under {Money(amount)}: {string.Join("; ", matchLines)}.",
            [AssistantToolNames.CatalogSearch],
            "catalog-public",
            false,
            AssistantResponseTypes.CatalogProducts,
            new AssistantCatalogProductsData(matches.Select(ToProductCard).ToArray(), amount));
    }

    private async Task<AssistantQueryResponse> CatalogGetProductAsync(
        Guid? productId,
        CancellationToken cancellationToken)
    {
        if (!productId.HasValue)
        {
            return Unsupported();
        }

        var product = await sender.Send(new GetProductByIdQuery(productId.Value), cancellationToken);

        if (product is null)
        {
            return new AssistantQueryResponse(
                "I could not find that product.",
                [AssistantToolNames.CatalogGetProduct],
                "catalog-public",
                false);
        }

        return new AssistantQueryResponse(
            $"{product.Name} ({product.Sku}) costs {Money(product.Price)} and is {(product.IsActive ? "active" : "inactive")}.",
            [AssistantToolNames.CatalogGetProduct],
            "catalog-public",
            false,
            AssistantResponseTypes.CatalogProduct,
            new AssistantCatalogProductData(ToProductCard(product)));
    }

    private static AssistantProductCardDto ToProductCard(ProductListItemDto product) =>
        new(
            product.ProductId,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.IsActive);

    private static AssistantProductCardDto ToProductCard(ProductDetailsDto product) =>
        new(
            product.ProductId,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.IsActive);

    private static string Money(decimal amount) =>
        amount.ToString("0.00", CultureInfo.InvariantCulture);
}
