using Ecommerce.Api.Assistant.TextToSql;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant;

public sealed class AssistantOrchestrator(
    IOrdersAssistantSubAgent ordersAssistantSubAgent,
    ICatalogAssistantSubAgent catalogAssistantSubAgent,
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
            AssistantIntentKind.CatalogProductsUnderPrice
                or AssistantIntentKind.CatalogGetProduct
                => await catalogAssistantSubAgent.HandleAsync(intent, cancellationToken),
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

}
