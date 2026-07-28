using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant;

public sealed class CatalogAssistantSubAgent(
    IAssistantLanguageModel languageModel,
    ICatalogAgentToolRegistry toolRegistry,
    AssistantSafetyPolicy safetyPolicy,
    IOptions<CatalogAgentOptions> options,
    ILogger<CatalogAssistantSubAgent> logger) : ICatalogAssistantSubAgent
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<AssistantQueryResponse> RunAsync(
        CatalogAgentRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null
            || string.IsNullOrWhiteSpace(request.UserMessage)
            || request.UserMessage.Length > 1000)
        {
            return InvalidInput();
        }

        if (!request.ExecutionContext.AllowedDataScopes.Contains(AssistantDataScopes.CatalogPublic, StringComparer.Ordinal)
            || safetyPolicy.IsUnsafeQuestion(request.UserMessage))
        {
            return Unsupported();
        }

        CatalogComparisonRequest? comparisonRequest = null;
        if (CatalogComparisonRequestParser.TryParse(request.UserMessage, out var parsedComparison))
        {
            if (safetyPolicy.IsUnsafeQuestion(parsedComparison.FirstSearchText)
                || safetyPolicy.IsUnsafeQuestion(parsedComparison.SecondSearchText))
            {
                return Unsupported();
            }

            comparisonRequest = parsedComparison;
        }

        if (!options.Value.Enabled)
        {
            return Failed("The bounded catalog agent is disabled.");
        }

        var stopwatch = Stopwatch.StartNew();
        var trustedProducts = new Dictionary<Guid, AssistantProductCardDto>();
        var detailedProductIds = new HashSet<Guid>();
        var toolsUsed = new List<string>();
        var searchesByText = new Dictionary<string, CatalogSearchToolResult>(StringComparer.OrdinalIgnoreCase);
        CatalogSearchToolResult? lastSearch = null;
        var messages = BuildInitialMessages(request);

        logger.LogInformation(
            "Catalog agent started: CorrelationId={CorrelationId}, AgentName={AgentName}.",
            request.ExecutionContext.CorrelationId,
            nameof(CatalogAssistantSubAgent));

        for (var iteration = 1; iteration <= options.Value.EffectiveMaximumIterations; iteration++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            AssistantModelResponse modelResponse;
            try
            {
                modelResponse = await languageModel.CompleteAsync(
                    new AssistantModelRequest(
                        CatalogAgentInstructions.Text,
                        LimitMessages(messages, request.UserMessage),
                        toolRegistry.Definitions,
                        AssistantResponseFormat.CatalogAgent),
                    cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                LogCompletion(request, iteration, "model_exception", stopwatch.ElapsedMilliseconds);
                return Failed("The catalog agent could not complete the request safely.", toolsUsed);
            }

            logger.LogInformation(
                "Catalog agent iteration: CorrelationId={CorrelationId}, AgentName={AgentName}, Iteration={Iteration}, ToolCallCount={ToolCallCount}, FinishReason={FinishReason}.",
                request.ExecutionContext.CorrelationId,
                nameof(CatalogAssistantSubAgent),
                iteration,
                modelResponse.ToolCalls.Count,
                modelResponse.FinishReason);

            if (modelResponse.FinishReason == AssistantModelFinishReason.ToolCallsRequested)
            {
                if (modelResponse.ToolCalls.Count is 0
                    || modelResponse.ToolCalls.Count > options.Value.EffectiveMaximumToolCallsPerIteration)
                {
                    LogCompletion(request, iteration, "tool_call_limit", stopwatch.ElapsedMilliseconds);
                    return Failed("The catalog agent requested an invalid number of tools.", toolsUsed);
                }

                foreach (var toolCall in modelResponse.ToolCalls)
                {
                    if (!toolRegistry.TryGetTool(toolCall.Name, out var tool) || tool is null)
                    {
                        logger.LogWarning(
                            "Catalog tool rejected: CorrelationId={CorrelationId}, AgentName={AgentName}, Iteration={Iteration}, ToolName={ToolName}, ToolOutcome={ToolOutcome}.",
                            request.ExecutionContext.CorrelationId,
                            nameof(CatalogAssistantSubAgent),
                            iteration,
                            toolCall.Name,
                            "tool_not_allowed");
                        return Failed("The catalog agent requested a tool that is not allowed.", toolsUsed);
                    }

                    messages.Add(new AssistantConversationMessage(
                        AssistantMessageRole.Assistant,
                        toolCall.ArgumentsJson,
                        toolCall.Id,
                        toolCall.Name));

                    var result = await tool.ExecuteAsync(
                        toolCall.ArgumentsJson,
                        new CatalogAgentToolExecutionContext(
                            request.ExecutionContext,
                            new HashSet<Guid>(trustedProducts.Keys)),
                        cancellationToken);

                    if (result.Succeeded)
                    {
                        if (!toolsUsed.Contains(result.ToolName, StringComparer.Ordinal))
                        {
                            toolsUsed.Add(result.ToolName);
                        }

                        CaptureTrustedProducts(
                            result,
                            trustedProducts,
                            detailedProductIds,
                            searchesByText,
                            ref lastSearch);
                    }

                    var resultCount = result.Data switch
                    {
                        CatalogSearchToolResult searchResult => searchResult.Products.Count,
                        CatalogProductToolResult => 1,
                        _ => 0
                    };

                    logger.LogInformation(
                        "Catalog tool completed: CorrelationId={CorrelationId}, AgentName={AgentName}, Iteration={Iteration}, ToolName={ToolName}, ToolOutcome={ToolOutcome}, ProductResultCount={ProductResultCount}.",
                        request.ExecutionContext.CorrelationId,
                        nameof(CatalogAssistantSubAgent),
                        iteration,
                        result.ToolName,
                        result.Succeeded ? "succeeded" : result.ErrorCode,
                        resultCount);

                    messages.Add(new AssistantConversationMessage(
                        AssistantMessageRole.Tool,
                        JsonSerializer.Serialize(result, JsonOptions),
                        toolCall.Id,
                        toolCall.Name));
                }

                continue;
            }

            if (modelResponse.FinishReason == AssistantModelFinishReason.Completed)
            {
                var response = BuildFinalResponse(
                    request.UserMessage,
                    modelResponse.FinalAnswer,
                    trustedProducts,
                    detailedProductIds,
                    toolsUsed,
                    lastSearch,
                    comparisonRequest,
                    searchesByText);
                LogCompletion(
                    request,
                    iteration,
                    response.Unsupported ? "failed" : "completed",
                    stopwatch.ElapsedMilliseconds);
                return response;
            }

            if (modelResponse.FinishReason == AssistantModelFinishReason.Refused)
            {
                LogCompletion(request, iteration, "refused", stopwatch.ElapsedMilliseconds);
                return toolsUsed.Count == 0
                    ? Unsupported()
                    : Failed("The catalog model refused to complete the request.", toolsUsed);
            }

            LogCompletion(request, iteration, modelResponse.FinishReason.ToString(), stopwatch.ElapsedMilliseconds);
            return Failed("The catalog model did not complete the request safely.", toolsUsed);
        }

        LogCompletion(
            request,
            options.Value.EffectiveMaximumIterations,
            "maximum_iterations_reached",
            stopwatch.ElapsedMilliseconds);
        return Failed("The catalog agent reached its safe execution limit.", toolsUsed);
    }

    private List<AssistantConversationMessage> BuildInitialMessages(CatalogAgentRequest request)
    {
        var messages = request.Conversation
            .Where(IsCatalogRelevant)
            .TakeLast(Math.Max(0, options.Value.EffectiveMaximumConversationMessages - 1))
            .ToList();
        messages.Add(new AssistantConversationMessage(AssistantMessageRole.User, request.UserMessage.Trim()));
        return messages;
    }

    private IReadOnlyCollection<AssistantConversationMessage> LimitMessages(
        IReadOnlyCollection<AssistantConversationMessage> messages,
        string userMessage)
    {
        var maximum = options.Value.EffectiveMaximumConversationMessages;
        var limited = messages.TakeLast(maximum).ToList();
        if (!limited.Any(message => message.Role == AssistantMessageRole.User
            && string.Equals(message.Content, userMessage.Trim(), StringComparison.Ordinal)))
        {
            if (limited.Count == maximum)
            {
                limited.RemoveAt(0);
            }

            limited.Insert(0, new AssistantConversationMessage(AssistantMessageRole.User, userMessage.Trim()));
        }

        return limited;
    }

    private static bool IsCatalogRelevant(AssistantConversationMessage message)
    {
        if (!string.IsNullOrWhiteSpace(message.ToolName))
        {
            return message.ToolName.StartsWith("catalog_", StringComparison.Ordinal);
        }

        var content = message.Content.ToLowerInvariant();
        return !content.Contains("order", StringComparison.Ordinal)
            && !content.Contains("authentication", StringComparison.Ordinal)
            && !content.Contains("token", StringComparison.Ordinal)
            && !content.Contains("admin", StringComparison.Ordinal);
    }

    private static void CaptureTrustedProducts(
        AssistantToolExecutionResult result,
        IDictionary<Guid, AssistantProductCardDto> trustedProducts,
        ISet<Guid> detailedProductIds,
        IDictionary<string, CatalogSearchToolResult> searchesByText,
        ref CatalogSearchToolResult? lastSearch)
    {
        if (result.Data is CatalogSearchToolResult searchResult)
        {
            lastSearch = searchResult;
            if (!string.IsNullOrWhiteSpace(searchResult.SearchText))
            {
                searchesByText[searchResult.SearchText.Trim()] = searchResult;
            }

            foreach (var product in searchResult.Products.Where(product => product.IsActive))
            {
                trustedProducts[product.ProductId] = product;
            }
        }

        if (result.Data is CatalogProductToolResult productResult && productResult.Product.IsActive)
        {
            trustedProducts[productResult.Product.ProductId] = productResult.Product;
            detailedProductIds.Add(productResult.Product.ProductId);
        }
    }

    private static AssistantQueryResponse BuildFinalResponse(
        string userMessage,
        CatalogAgentFinalAnswer? finalAnswer,
        IReadOnlyDictionary<Guid, AssistantProductCardDto> trustedProducts,
        IReadOnlySet<Guid> detailedProductIds,
        IReadOnlyCollection<string> toolsUsed,
        CatalogSearchToolResult? lastSearch,
        CatalogComparisonRequest? comparisonRequest,
        IReadOnlyDictionary<string, CatalogSearchToolResult> searchesByText)
    {
        if (finalAnswer is null || toolsUsed.Count == 0)
        {
            return Failed("The catalog agent returned an invalid final response.", toolsUsed);
        }

        var selected = new List<AssistantProductCardDto>();
        foreach (var productId in finalAnswer.SelectedProductIds.Distinct())
        {
            if (!trustedProducts.TryGetValue(productId, out var product) || !product.IsActive)
            {
                return Failed("The catalog agent referenced an untrusted product.", toolsUsed);
            }

            selected.Add(product);
        }

        if (comparisonRequest is not null)
        {
            return BuildComparisonResponse(
                comparisonRequest,
                finalAnswer,
                selected,
                detailedProductIds,
                toolsUsed,
                searchesByText);
        }

        if (finalAnswer.ResponseType == CatalogAgentFinalResponseType.CatalogProduct
            && (selected.Count != 1 || !detailedProductIds.Contains(selected[0].ProductId)))
        {
            return Failed("Full product details require a trusted detail-tool result.", toolsUsed);
        }

        if (finalAnswer.ResponseType == CatalogAgentFinalResponseType.Text && selected.Count > 0)
        {
            return Failed("The catalog agent returned inconsistent final product selection.", toolsUsed);
        }

        if (!ValidateRequestedSelection(userMessage, selected, lastSearch))
        {
            return Failed("The catalog agent selected a product inconsistent with verified prices.", toolsUsed);
        }

        if (selected.Count == 0)
        {
            if (lastSearch is null || lastSearch.Products.Count > 0)
            {
                return Failed("The catalog agent did not select trusted product data.", toolsUsed);
            }

            return new AssistantQueryResponse(
                "I did not find active products matching that catalog goal in the returned catalog page.",
                toolsUsed,
                AssistantDataScopes.CatalogPublic,
                false,
                AssistantResponseTypes.CatalogProducts,
                new AssistantCatalogProductsData(Array.Empty<AssistantProductCardDto>(), finalAnswer.MaximumPrice ?? lastSearch.MaximumPrice));
        }

        if (finalAnswer.ResponseType == CatalogAgentFinalResponseType.CatalogProduct)
        {
            var product = selected[0];
            var prefix = userMessage.Contains("cheapest", StringComparison.OrdinalIgnoreCase)
                ? "The cheapest verified product in the returned catalog page is"
                : userMessage.Contains("most expensive", StringComparison.OrdinalIgnoreCase)
                    ? "The most expensive verified product in the returned catalog page is"
                    : "The verified product is";
            return new AssistantQueryResponse(
                $"{prefix} {product.Name} ({product.Sku}) at {Money(product.Price)}.",
                toolsUsed,
                AssistantDataScopes.CatalogPublic,
                false,
                AssistantResponseTypes.CatalogProduct,
                new AssistantCatalogProductData(product));
        }

        var lines = selected.Select(product => $"{product.Name} ({product.Sku}) {Money(product.Price)}").ToArray();
        var answer = finalAnswer.NeedsClarification
            ? $"I found multiple verified active products: {string.Join("; ", lines)}. Please choose an exact SKU or product."
            : $"Verified active products from the returned catalog page: {string.Join("; ", lines)}.";
        return new AssistantQueryResponse(
            answer,
            toolsUsed,
            AssistantDataScopes.CatalogPublic,
            false,
            AssistantResponseTypes.CatalogProducts,
            new AssistantCatalogProductsData(selected, finalAnswer.MaximumPrice ?? lastSearch?.MaximumPrice));
    }

    private static AssistantQueryResponse BuildComparisonResponse(
        CatalogComparisonRequest comparison,
        CatalogAgentFinalAnswer finalAnswer,
        IReadOnlyCollection<AssistantProductCardDto> selected,
        IReadOnlySet<Guid> detailedProductIds,
        IReadOnlyCollection<string> toolsUsed,
        IReadOnlyDictionary<string, CatalogSearchToolResult> searchesByText)
    {
        if (finalAnswer.ResponseType != CatalogAgentFinalResponseType.CatalogProducts
            || !searchesByText.TryGetValue(comparison.FirstSearchText, out var firstSearch)
            || !searchesByText.TryGetValue(comparison.SecondSearchText, out var secondSearch)
            || !IsValidComparisonSearch(firstSearch)
            || !IsValidComparisonSearch(secondSearch))
        {
            return Failed("The catalog agent did not verify both comparison searches.", toolsUsed);
        }

        var missingTerms = new List<string>();
        if (firstSearch.TotalCount == 0)
        {
            missingTerms.Add(comparison.FirstSearchText);
        }

        if (secondSearch.TotalCount == 0)
        {
            missingTerms.Add(comparison.SecondSearchText);
        }

        if (missingTerms.Count > 0)
        {
            if (selected.Count > 0)
            {
                return Failed("The catalog agent selected a product for an incomplete comparison.", toolsUsed);
            }

            return new AssistantQueryResponse(
                $"I did not find an active product matching {string.Join(" or ", missingTerms.Select(Quote))}, so I cannot compare the two products.",
                toolsUsed,
                AssistantDataScopes.CatalogPublic,
                false,
                AssistantResponseTypes.CatalogProducts,
                new AssistantCatalogProductsData(Array.Empty<AssistantProductCardDto>(), null));
        }

        var ambiguousSearches = new List<(string Term, CatalogSearchToolResult Search)>();
        if (firstSearch.TotalCount > 1)
        {
            ambiguousSearches.Add((comparison.FirstSearchText, firstSearch));
        }

        if (secondSearch.TotalCount > 1)
        {
            ambiguousSearches.Add((comparison.SecondSearchText, secondSearch));
        }

        if (ambiguousSearches.Count > 0)
        {
            if (ambiguousSearches.Any(item => item.Search.Products.Count < 2))
            {
                return Failed("The catalog search choices were inconsistent with their verified counts.", toolsUsed);
            }

            var choices = ambiguousSearches
                .SelectMany(item => item.Search.Products)
                .Where(product => product.IsActive)
                .DistinctBy(product => product.ProductId)
                .ToArray();
            if (!finalAnswer.NeedsClarification
                || choices.Length == 0
                || !IdsMatch(selected, choices))
            {
                return Failed("The catalog agent did not return the verified ambiguous choices.", toolsUsed);
            }

            var groups = ambiguousSearches.Select(item =>
                $"{Quote(item.Term)} matched {string.Join("; ", item.Search.Products.Select(FormatChoice))}");
            return new AssistantQueryResponse(
                $"I found multiple active products, so I will not guess: {string.Join(". ", groups)}. Please choose an exact SKU.",
                toolsUsed,
                AssistantDataScopes.CatalogPublic,
                false,
                AssistantResponseTypes.CatalogProducts,
                new AssistantCatalogProductsData(choices, null));
        }

        if (firstSearch.Products.Count != 1 || secondSearch.Products.Count != 1)
        {
            return Failed("The catalog search results were inconsistent with their verified counts.", toolsUsed);
        }

        var firstMatch = firstSearch.Products.Single();
        var secondMatch = secondSearch.Products.Single();
        if (firstMatch.ProductId == secondMatch.ProductId)
        {
            if (!finalAnswer.NeedsClarification
                || !IdsMatch(selected, [firstMatch]))
            {
                return Failed("The catalog agent did not clarify the duplicate comparison product.", toolsUsed);
            }

            return new AssistantQueryResponse(
                $"{Quote(comparison.FirstSearchText)} and {Quote(comparison.SecondSearchText)} both resolve to {FormatChoice(firstMatch)}. Please choose a different second product.",
                toolsUsed,
                AssistantDataScopes.CatalogPublic,
                false,
                AssistantResponseTypes.CatalogProducts,
                new AssistantCatalogProductsData([firstMatch], null));
        }

        var expectedMatches = new[] { firstMatch, secondMatch };
        if (finalAnswer.NeedsClarification
            || !IdsMatch(selected, expectedMatches)
            || expectedMatches.Any(product => !detailedProductIds.Contains(product.ProductId)))
        {
            return Failed("The catalog agent did not load both trusted product details.", toolsUsed);
        }

        var detailedById = selected.ToDictionary(product => product.ProductId);
        var first = detailedById[firstMatch.ProductId];
        var second = detailedById[secondMatch.ProductId];
        var answer = BuildComparisonAnswer(first, second, comparison.Mode);
        return new AssistantQueryResponse(
            answer,
            toolsUsed,
            AssistantDataScopes.CatalogPublic,
            false,
            AssistantResponseTypes.CatalogProducts,
            new AssistantCatalogProductsData([first, second], null));
    }

    private static bool IsValidComparisonSearch(CatalogSearchToolResult search) =>
        search.PageNumber == 1
        && search.PageSize == 2
        && search.MaximumPrice is null
        && !string.IsNullOrWhiteSpace(search.SearchText);

    private static bool IdsMatch(
        IReadOnlyCollection<AssistantProductCardDto> actual,
        IReadOnlyCollection<AssistantProductCardDto> expected)
    {
        var actualIds = actual.Select(product => product.ProductId).OrderBy(id => id).ToArray();
        var expectedIds = expected.Select(product => product.ProductId).Distinct().OrderBy(id => id).ToArray();
        return actualIds.SequenceEqual(expectedIds);
    }

    private static string BuildComparisonAnswer(
        AssistantProductCardDto first,
        AssistantProductCardDto second,
        CatalogComparisonMode mode)
    {
        if (first.Price == second.Price)
        {
            return $"{FormatChoice(first)} and {FormatChoice(second)} have the same verified price, so neither is cheaper. Both products are active; their public descriptions are included in the product data.";
        }

        var cheaper = first.Price < second.Price ? first : second;
        var expensive = first.Price < second.Price ? second : first;
        var difference = expensive.Price - cheaper.Price;
        if (mode == CatalogComparisonMode.Cheaper)
        {
            return $"{FormatChoice(cheaper)} is cheaper than {FormatChoice(expensive)} by {Money(difference)}. Both products are active.";
        }

        return $"{FormatChoice(first)} compared with {FormatChoice(second)}: {cheaper.Name} is cheaper by {Money(difference)}. Both products are active; their public descriptions are included in the product data.";
    }

    private static string FormatChoice(AssistantProductCardDto product) =>
        $"{product.Name} ({product.Sku}) at {Money(product.Price)}";

    private static string Quote(string value) => $"\"{value}\"";

    private static bool ValidateRequestedSelection(
        string userMessage,
        IReadOnlyCollection<AssistantProductCardDto> selected,
        CatalogSearchToolResult? lastSearch)
    {
        var requestsCheapest = userMessage.Contains("cheapest", StringComparison.OrdinalIgnoreCase);
        var requestsMostExpensive = userMessage.Contains("most expensive", StringComparison.OrdinalIgnoreCase);
        if (!requestsCheapest && !requestsMostExpensive)
        {
            return true;
        }

        if (selected.Count != 1 || lastSearch is null || lastSearch.Products.Count == 0)
        {
            return false;
        }

        if (requestsCheapest)
        {
            return selected.First().Price == lastSearch.Products.Min(product => product.Price);
        }

        if (userMessage.Contains("most expensive", StringComparison.OrdinalIgnoreCase))
        {
            return selected.First().Price == lastSearch.Products.Max(product => product.Price);
        }

        return true;
    }

    private void LogCompletion(
        CatalogAgentRequest request,
        int iteration,
        string outcome,
        long durationMilliseconds) =>
        logger.LogInformation(
            "Catalog agent completed: CorrelationId={CorrelationId}, AgentName={AgentName}, Iteration={Iteration}, FinalOutcome={FinalOutcome}, DurationMilliseconds={DurationMilliseconds}.",
            request.ExecutionContext.CorrelationId,
            nameof(CatalogAssistantSubAgent),
            iteration,
            outcome,
            durationMilliseconds);

    private static AssistantQueryResponse InvalidInput() =>
        new("A catalog question is required.", Array.Empty<string>(), AssistantDataScopes.None, true);

    private static AssistantQueryResponse Unsupported() =>
        new("I can help only with read-only public catalog requests.", Array.Empty<string>(), AssistantDataScopes.None, true);

    private static AssistantQueryResponse Failed(
        string message,
        IReadOnlyCollection<string>? toolsUsed = null) =>
        new(
            message,
            toolsUsed ?? Array.Empty<string>(),
            toolsUsed is { Count: > 0 } ? AssistantDataScopes.CatalogPublic : AssistantDataScopes.None,
            true);

    private static string Money(decimal amount) =>
        amount.ToString("0.00", CultureInfo.InvariantCulture);
}
