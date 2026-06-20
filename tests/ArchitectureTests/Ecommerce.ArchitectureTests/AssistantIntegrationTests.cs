using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Ecommerce.Api.Assistant;
using Ecommerce.Api.Controllers.Assistant;
using CatalogPagedResult = Ecommerce.Catalog.Application.Abstractions.PagedResult<Ecommerce.Catalog.Application.Products.SearchProducts.ProductListItemDto>;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NetArchTest.Rules;
using OrdersPagedResult = Ecommerce.Orders.Application.Abstractions.PagedResult<Ecommerce.Orders.Application.Orders.ListOrdersForBuyer.OrderSummaryDto>;

namespace Ecommerce.ArchitectureTests;

public sealed class AssistantIntegrationTests
{
    [Fact]
    public void AssistantToolRegistry_ShouldExposeOnlyApprovedReadOnlyAllowlist()
    {
        var tools = new AssistantToolRegistry().GetAllowedTools().OrderBy(tool => tool).ToArray();

        Assert.Equal(
            new[]
            {
                "catalog_get_product",
                "catalog_search",
                "orders_analyze",
                "orders_get_order",
                "orders_search"
            },
            tools);
    }

    [Theory]
    [InlineData("Show my recent orders", AssistantIntentKind.RecentOrders)]
    [InlineData("What products did I order?", AssistantIntentKind.ProductsOrdered)]
    [InlineData("Which orders contain product 4444?", AssistantIntentKind.OrdersContainingProduct)]
    [InlineData("What is my total spend?", AssistantIntentKind.TotalSpend)]
    [InlineData("What did I buy most often?", AssistantIntentKind.ProductFrequency)]
    [InlineData("Find products under 20", AssistantIntentKind.CatalogProductsUnderPrice)]
    [InlineData("Find orders containing products over 10", AssistantIntentKind.OrdersContainingProductsOverAmount)]
    public void IntentRouter_ShouldRouteSupportedQuestions(string question, AssistantIntentKind expectedKind)
    {
        var intent = new AssistantIntentRouter().Route(question);

        Assert.Equal(expectedKind, intent.Kind);
    }

    [Fact]
    public void IntentRouter_ShouldExtractProductSkuNameSearchText()
    {
        var intent = new AssistantIntentRouter().Route("Which orders contain product/SKU/name 4444?");

        Assert.Equal(AssistantIntentKind.OrdersContainingProduct, intent.Kind);
        Assert.Equal("4444", intent.SearchText);
    }

    [Fact]
    public async Task DeterministicIntentInterpreter_ShouldBeDefaultInterpreterShape()
    {
        var interpreter = new DeterministicAssistantIntentInterpreter(new AssistantIntentRouter());

        var plan = await interpreter.InterpretAsync("Show my recent orders", CancellationToken.None);

        Assert.NotNull(plan);
        Assert.Equal(AssistantIntentKind.RecentOrders, plan.Kind);
        Assert.Equal(new[] { AssistantToolNames.OrdersSearch }, plan.Tools);
        Assert.Empty(plan.Arguments);
    }

    [Fact]
    public void Program_ShouldRegisterLlmInterpreterBehindConfiguration()
    {
        var root = ProjectGraph.GetRootPath();
        var source = File.ReadAllText(Path.Combine(root, "src", "Api", "Ecommerce.Api", "Program.cs"));

        Assert.Contains("Configure<AssistantLlmOptions>", source, StringComparison.Ordinal);
        Assert.Contains("AddScoped<IAssistantIntentInterpreter>", source, StringComparison.Ordinal);
        Assert.Contains("AddHttpClient<IAssistantLlmClient, HttpAssistantLlmClient>", source, StringComparison.Ordinal);
        Assert.Contains("LlmAssistantIntentInterpreter", source, StringComparison.Ordinal);
        Assert.Contains("DeterministicAssistantIntentInterpreter", source, StringComparison.Ordinal);
        Assert.Contains("options.Enabled", source, StringComparison.Ordinal);
    }

    [Fact]
    public async Task LlmInterpreter_DisabledMode_ShouldReturnNoPlanWithoutSecret()
    {
        var client = new RecordingLlmClient("""{"kind":"RecentOrders","tools":["orders_search"],"arguments":{}}""");
        var interpreter = CreateLlmInterpreter(client, enabled: false);

        var plan = await interpreter.InterpretAsync(
            "Could you remind me what I bought lately?",
            CancellationToken.None);

        Assert.Null(plan);
        Assert.Empty(client.Questions);
    }

    [Fact]
    public async Task HttpLlmClient_MissingSecret_ShouldReturnNoPlanWithoutSendingRequest()
    {
        var missingSecretName = "ECOMMERCE_ASSISTANT_TEST_MISSING_SECRET_" + Guid.NewGuid().ToString("N");
        var handler = new CountingHttpMessageHandler();
        var client = new HttpAssistantLlmClient(
            new HttpClient(handler),
            Options.Create(new AssistantLlmOptions
            {
                Enabled = true,
                Endpoint = "https://example.test/v1/responses",
                Model = "test-model",
                ApiKeyEnvironmentVariable = missingSecretName
            }),
            NullLogger<HttpAssistantLlmClient>.Instance);

        var planJson = await client.CreateIntentPlanJsonAsync(
            "Show my recent orders",
            CancellationToken.None);

        Assert.Null(planJson);
        Assert.Equal(0, handler.SendCount);
    }

    [Fact]
    public async Task HttpLlmClient_ShouldSendResponsesApiRequestShape()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {"output":[{"type":"message","content":[{"type":"output_text","text":"{\"kind\":\"RecentOrders\",\"tools\":[\"orders_search\"],\"arguments\":{}}"}]}]}
                """,
                Encoding.UTF8,
                "application/json")
        });
        var client = new HttpAssistantLlmClient(
            new HttpClient(handler),
            Options.Create(CreateEnabledLlmOptions()),
            NullLogger<HttpAssistantLlmClient>.Instance);

        var planJson = await client.CreateIntentPlanJsonAsync(
            "Show my recent orders",
            CancellationToken.None);

        Assert.Equal("""{"kind":"RecentOrders","tools":["orders_search"],"arguments":{}}""", planJson);
        Assert.NotNull(handler.RequestBody);

        using var document = JsonDocument.Parse(handler.RequestBody);
        Assert.True(document.RootElement.TryGetProperty("model", out var model));
        Assert.Equal("test-model", model.GetString());
        Assert.True(document.RootElement.TryGetProperty("input", out var input));
        Assert.Equal(JsonValueKind.Array, input.ValueKind);
        Assert.True(document.RootElement.TryGetProperty("text", out var text));
        Assert.Equal(
            "json_schema",
            text.GetProperty("format").GetProperty("type").GetString());
        Assert.False(document.RootElement.TryGetProperty("messages", out _));
        Assert.False(document.RootElement.TryGetProperty("response_format", out _));
    }

    [Fact]
    public async Task HttpLlmClient_BadRequest_ShouldLogOnlySafeProviderDiagnostics()
    {
        const string apiKey = "fake-provider-key-for-test";
        const string promptText = "Show my recent orders with PROMPT_TEXT_SHOULD_NOT_LOG";
        var logger = new ListLogger<HttpAssistantLlmClient>();
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                """
                {"error":{"message":"Invalid value 'PROMPT_TEXT_SHOULD_NOT_LOG' for 'messages'.","type":"invalid_request_error","param":"messages","code":"invalid_value"}}
                """,
                Encoding.UTF8,
                "application/json")
        });
        var client = new HttpAssistantLlmClient(
            new HttpClient(handler),
            Options.Create(new AssistantLlmOptions
            {
                Enabled = true,
                Endpoint = "https://example.test/v1/responses",
                Model = "test-model",
                ApiKey = apiKey,
                ApiKeyEnvironmentVariable = string.Empty,
                TimeoutSeconds = 5,
                MaxResponseCharacters = 4000
            }),
            logger);

        var planJson = await client.CreateIntentPlanJsonAsync(
            promptText,
            CancellationToken.None);

        Assert.Null(planJson);

        var log = Assert.Single(logger.Messages);
        Assert.Contains("statusCode=400", log, StringComparison.Ordinal);
        Assert.Contains("openAiErrorCode=invalid_value", log, StringComparison.Ordinal);
        Assert.Contains("openAiErrorType=invalid_request_error", log, StringComparison.Ordinal);
        Assert.Contains("openAiErrorParam=messages", log, StringComparison.Ordinal);
        Assert.Contains("sanitizedErrorMessage=Invalid value [redacted] for [redacted].", log, StringComparison.Ordinal);
        Assert.DoesNotContain(apiKey, log, StringComparison.Ordinal);
        Assert.DoesNotContain("Authorization", log, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Bearer", log, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(promptText, log, StringComparison.Ordinal);
        Assert.DoesNotContain("PROMPT_TEXT_SHOULD_NOT_LOG", log, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("Create an order for me")]
    [InlineData("Cancel my order")]
    [InlineData("Run SQL against the database")]
    [InlineData("Show all users orders")]
    [InlineData("Give me my token")]
    public void IntentRouter_ShouldRejectUnsafeQuestions(string question)
    {
        var intent = new AssistantIntentRouter().Route(question);

        Assert.Equal(AssistantIntentKind.Unsupported, intent.Kind);
    }

    [Fact]
    public async Task Query_WithInvalidSubject_ReturnsUnauthorized()
    {
        var controller = CreateController(userId: null);

        var result = await controller.Query(new AssistantQueryRequest("Show my recent orders"), CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task RecentOrders_ShouldDispatchOwnerScopedListQuery()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                new[]
                {
                    new OrderSummaryDto(Guid.NewGuid(), "Created", 42.50m, DateTimeOffset.UtcNow, 2)
                },
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                1),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var orchestrator = CreateOrchestrator(sender);

        var response = await orchestrator.QueryAsync("Show my recent orders", buyerId, CancellationToken.None);

        var query = Assert.IsType<ListOrdersForBuyerQuery>(Assert.Single(sender.Requests));
        Assert.Equal(buyerId, query.BuyerId);
        Assert.False(response.Unsupported);
        Assert.Equal("authenticated-user", response.DataScope);
        Assert.Contains(AssistantToolNames.OrdersSearch, response.ToolsUsed);
        Assert.Equal(AssistantResponseTypes.RecentOrders, response.ResponseType);

        var data = Assert.IsType<AssistantOrdersData>(response.Data);
        var order = Assert.Single(data.Orders);
        Assert.Equal("Created", order.Status);
        Assert.Equal(42.50m, order.TotalAmount);
        Assert.Equal(2, order.LineCount);
    }

    [Fact]
    public async Task FakeInterpreterPlan_ShouldRouteFlexiblePhrasingThroughValidation()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                new[]
                {
                    new OrderSummaryDto(Guid.NewGuid(), "Created", 32.10m, DateTimeOffset.UtcNow, 1)
                },
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                1),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var interpreter = new StaticPlanInterpreter(new AssistantIntentPlan(
            AssistantIntentKind.RecentOrders,
            [AssistantToolNames.OrdersSearch],
            AssistantIntentPlan.EmptyArguments()));
        var orchestrator = CreateOrchestrator(sender, interpreter);

        var response = await orchestrator.QueryAsync(
            "Could you remind me what I grabbed lately?",
            buyerId,
            CancellationToken.None);

        var query = Assert.IsType<ListOrdersForBuyerQuery>(Assert.Single(sender.Requests));
        Assert.Equal(buyerId, query.BuyerId);
        Assert.False(response.Unsupported);
        Assert.Contains(AssistantToolNames.OrdersSearch, response.ToolsUsed);
    }

    [Fact]
    public async Task FakeLlmProviderPlan_ShouldRouteFlexiblePhrasingThroughValidation()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                new[]
                {
                    new OrderSummaryDto(Guid.NewGuid(), "Created", 19.95m, DateTimeOffset.UtcNow, 1)
                },
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                1),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var llmInterpreter = CreateLlmInterpreter(new RecordingLlmClient(
            """{"kind":"RecentOrders","tools":["orders_search"],"arguments":{}}"""));
        var orchestrator = CreateOrchestrator(sender, llmInterpreter);

        var response = await orchestrator.QueryAsync(
            "Could you recap my latest shopping activity?",
            buyerId,
            CancellationToken.None);

        var query = Assert.IsType<ListOrdersForBuyerQuery>(Assert.Single(sender.Requests));
        Assert.Equal(buyerId, query.BuyerId);
        Assert.False(response.Unsupported);
        Assert.Contains(AssistantToolNames.OrdersSearch, response.ToolsUsed);
    }

    [Theory]
    [MemberData(nameof(InvalidModelPlans))]
    public async Task InvalidInterpreterPlans_ShouldFailClosedWithoutDispatch(AssistantIntentPlan plan)
    {
        var sender = new RecordingSender(_ => throw new InvalidOperationException("Sender should not be called."));
        var orchestrator = CreateOrchestrator(sender, new StaticPlanInterpreter(plan));

        var response = await orchestrator.QueryAsync("Show my recent orders", Guid.NewGuid(), CancellationToken.None);

        Assert.True(response.Unsupported);
        Assert.Empty(response.ToolsUsed);
        Assert.Empty(sender.Requests);
    }

    [Fact]
    public async Task MalformedLlmOutput_ShouldFallBackToDeterministicInterpreter()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                Array.Empty<OrderSummaryDto>(),
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                0),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var orchestrator = CreateOrchestrator(
            sender,
            CreateLlmInterpreter(new RecordingLlmClient("not json")));

        var response = await orchestrator.QueryAsync("Show my recent orders", buyerId, CancellationToken.None);

        var query = Assert.IsType<ListOrdersForBuyerQuery>(Assert.Single(sender.Requests));
        Assert.Equal(buyerId, query.BuyerId);
        Assert.False(response.Unsupported);
    }

    [Fact]
    public async Task LlmProviderFailure_ShouldFallBackToDeterministicInterpreter()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                Array.Empty<OrderSummaryDto>(),
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                0),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var orchestrator = CreateOrchestrator(sender, CreateLlmInterpreter(new ThrowingLlmClient()));

        var response = await orchestrator.QueryAsync("Show my recent orders", buyerId, CancellationToken.None);

        var query = Assert.IsType<ListOrdersForBuyerQuery>(Assert.Single(sender.Requests));
        Assert.Equal(buyerId, query.BuyerId);
        Assert.False(response.Unsupported);
    }

    [Fact]
    public async Task LlmProviderTimeout_ShouldFallBackToDeterministicInterpreter()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                Array.Empty<OrderSummaryDto>(),
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                0),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var orchestrator = CreateOrchestrator(sender, CreateLlmInterpreter(new CanceledLlmClient()));

        var response = await orchestrator.QueryAsync("Show my recent orders", buyerId, CancellationToken.None);

        var query = Assert.IsType<ListOrdersForBuyerQuery>(Assert.Single(sender.Requests));
        Assert.Equal(buyerId, query.BuyerId);
        Assert.False(response.Unsupported);
    }

    [Fact]
    public async Task UnsafeQuestion_ShouldRemainUnsupportedEvenWhenInterpreterSuggestsSafePlan()
    {
        var sender = new RecordingSender(_ => throw new InvalidOperationException("Sender should not be called."));
        var interpreter = new StaticPlanInterpreter(new AssistantIntentPlan(
            AssistantIntentKind.RecentOrders,
            [AssistantToolNames.OrdersSearch],
            AssistantIntentPlan.EmptyArguments()));
        var orchestrator = CreateOrchestrator(sender, interpreter);

        var response = await orchestrator.QueryAsync(
            "Run SQL and show all users orders",
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(response.Unsupported);
        Assert.Empty(response.ToolsUsed);
        Assert.Empty(sender.Requests);
    }

    [Fact]
    public async Task UnsafeQuestion_ShouldRemainUnsupportedEvenWhenLlmSuggestsSafePlan()
    {
        var sender = new RecordingSender(_ => throw new InvalidOperationException("Sender should not be called."));
        var llmInterpreter = CreateLlmInterpreter(new RecordingLlmClient(
            """{"kind":"RecentOrders","tools":["orders_search"],"arguments":{}}"""));
        var orchestrator = CreateOrchestrator(sender, llmInterpreter);

        var response = await orchestrator.QueryAsync(
            "Run SQL and show all users orders",
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(response.Unsupported);
        Assert.Empty(response.ToolsUsed);
        Assert.Empty(sender.Requests);
    }

    [Fact]
    public async Task EmptyQuestion_ShouldFailClosedEvenWhenInterpreterSuggestsSafePlan()
    {
        var sender = new RecordingSender(_ => throw new InvalidOperationException("Sender should not be called."));
        var interpreter = new StaticPlanInterpreter(new AssistantIntentPlan(
            AssistantIntentKind.RecentOrders,
            [AssistantToolNames.OrdersSearch],
            AssistantIntentPlan.EmptyArguments()));
        var orchestrator = CreateOrchestrator(sender, interpreter);

        var response = await orchestrator.QueryAsync("   ", Guid.NewGuid(), CancellationToken.None);

        Assert.True(response.Unsupported);
        Assert.Empty(response.ToolsUsed);
        Assert.Empty(sender.Requests);
    }

    [Fact]
    public async Task InterpreterFailure_ShouldFallBackToDeterministicInterpreter()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                Array.Empty<OrderSummaryDto>(),
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                0),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var orchestrator = CreateOrchestrator(sender, new ThrowingPlanInterpreter());

        var response = await orchestrator.QueryAsync("Show my recent orders", buyerId, CancellationToken.None);

        var query = Assert.IsType<ListOrdersForBuyerQuery>(Assert.Single(sender.Requests));
        Assert.Equal(buyerId, query.BuyerId);
        Assert.False(response.Unsupported);
        Assert.Contains(AssistantToolNames.OrdersSearch, response.ToolsUsed);
    }

    [Fact]
    public async Task ProductsOrdered_ShouldLoadOwnedOrderDetailsWithAuthenticatedBuyer()
    {
        var buyerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                new[] { new OrderSummaryDto(orderId, "Created", 15m, DateTimeOffset.UtcNow, 1) },
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                1),
            GetOrderByIdQuery query => new OrderDetailsDto(
                query.OrderId,
                query.BuyerId,
                "Created",
                15m,
                DateTimeOffset.UtcNow,
                new[]
                {
                    new OrderLineDetailsDto(Guid.NewGuid(), Guid.NewGuid(), "SKU-1", "Tea", 5m, 3, 15m)
                }),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var orchestrator = CreateOrchestrator(sender);

        var response = await orchestrator.QueryAsync("What products did I order?", buyerId, CancellationToken.None);

        Assert.Contains(sender.Requests.OfType<ListOrdersForBuyerQuery>(), query => query.BuyerId == buyerId);
        Assert.Contains(sender.Requests.OfType<GetOrderByIdQuery>(), query => query.BuyerId == buyerId);
        Assert.False(response.Unsupported);
        Assert.Contains(AssistantToolNames.OrdersAnalyze, response.ToolsUsed);
        Assert.Contains("Tea", response.Answer, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(AssistantResponseTypes.OrderedProducts, response.ResponseType);

        var data = Assert.IsType<AssistantOrderedProductsData>(response.Data);
        var product = Assert.Single(data.Products);
        Assert.Equal("SKU-1", product.ProductSku);
        Assert.Equal("Tea", product.ProductName);
        Assert.Equal(3, product.Quantity);
    }

    [Fact]
    public async Task CatalogProductsUnderPrice_ShouldUseCatalogSearchOnly()
    {
        var sender = new RecordingSender(request => request switch
        {
            SearchProductsQuery query => new CatalogPagedResult(
                new[]
                {
                    new ProductListItemDto(Guid.NewGuid(), "SKU-1", "Tea", null, true, DateTimeOffset.UtcNow)
                    {
                        Price = 9.99m
                    },
                    new ProductListItemDto(Guid.NewGuid(), "SKU-2", "Cake", null, true, DateTimeOffset.UtcNow)
                    {
                        Price = 25m
                    }
                },
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                2),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var orchestrator = CreateOrchestrator(sender);

        var response = await orchestrator.QueryAsync("Find products under 20", Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<SearchProductsQuery>(Assert.Single(sender.Requests));
        Assert.Equal("catalog-public", response.DataScope);
        Assert.Contains(AssistantToolNames.CatalogSearch, response.ToolsUsed);
        Assert.Contains("Tea", response.Answer, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Cake", response.Answer, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(AssistantResponseTypes.CatalogProducts, response.ResponseType);

        var data = Assert.IsType<AssistantCatalogProductsData>(response.Data);
        var product = Assert.Single(data.Products);
        Assert.Equal("SKU-1", product.Sku);
        Assert.Equal("Tea", product.Name);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal(20m, data.MaxPrice);
    }

    [Fact]
    public async Task CatalogGetProduct_ShouldReturnStructuredProductData()
    {
        var productId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            GetProductByIdQuery query when query.ProductId == productId => new ProductDetailsDto(
                productId,
                "SKU-1",
                "Tea",
                "Green tea",
                true,
                DateTimeOffset.UtcNow,
                null)
            {
                Price = 9.99m
            },
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var interpreter = new StaticPlanInterpreter(new AssistantIntentPlan(
            AssistantIntentKind.CatalogGetProduct,
            [AssistantToolNames.CatalogGetProduct],
            new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                ["productId"] = productId.ToString()
            }));
        var orchestrator = CreateOrchestrator(sender, interpreter);

        var response = await orchestrator.QueryAsync("Show product details", Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(AssistantResponseTypes.CatalogProduct, response.ResponseType);
        Assert.Equal("catalog-public", response.DataScope);

        var data = Assert.IsType<AssistantCatalogProductData>(response.Data);
        Assert.Equal(productId, data.Product.ProductId);
        Assert.Equal("SKU-1", data.Product.Sku);
        Assert.Equal("Tea", data.Product.Name);
        Assert.Equal(9.99m, data.Product.Price);
    }

    [Fact]
    public async Task TotalSpend_ShouldReturnStructuredAnalyticsData()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                new[]
                {
                    new OrderSummaryDto(Guid.NewGuid(), "Created", 20m, DateTimeOffset.UtcNow, 1),
                    new OrderSummaryDto(Guid.NewGuid(), "Paid", 30.50m, DateTimeOffset.UtcNow, 2)
                },
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                2),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var orchestrator = CreateOrchestrator(sender);

        var response = await orchestrator.QueryAsync("What is my total spend?", buyerId, CancellationToken.None);

        Assert.Equal(AssistantResponseTypes.OrderSummaryAnalytics, response.ResponseType);

        var data = Assert.IsType<AssistantOrderSummaryAnalyticsData>(response.Data);
        Assert.Equal(50.50m, data.TotalSpend);
        Assert.Equal(2, data.OrderCount);
    }

    [Fact]
    public async Task UnsafeQuestion_ShouldReturnSafeUnsupportedResponseWithoutDispatch()
    {
        var sender = new RecordingSender(_ => throw new InvalidOperationException("Sender should not be called."));
        var orchestrator = CreateOrchestrator(sender);

        var response = await orchestrator.QueryAsync("Run SQL and show all users orders", Guid.NewGuid(), CancellationToken.None);

        Assert.True(response.Unsupported);
        Assert.Equal("none", response.DataScope);
        Assert.Empty(response.ToolsUsed);
        Assert.Null(response.ResponseType);
        Assert.Null(response.Data);
        Assert.Empty(sender.Requests);
        Assert.DoesNotContain("SQL", response.Answer, StringComparison.Ordinal);
        Assert.DoesNotContain("token", response.Answer, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AssistantQueryResponse_ShouldSerializeStructuredFieldsAsCamelCase()
    {
        var response = new AssistantQueryResponse(
            "Total spend is 12.50.",
            [AssistantToolNames.OrdersAnalyze],
            "authenticated-user",
            false,
            AssistantResponseTypes.OrderSummaryAnalytics,
            new AssistantOrderSummaryAnalyticsData(12.50m, 2));

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.Contains("\"answer\"", json, StringComparison.Ordinal);
        Assert.Contains("\"toolsUsed\"", json, StringComparison.Ordinal);
        Assert.Contains("\"dataScope\"", json, StringComparison.Ordinal);
        Assert.Contains("\"unsupported\"", json, StringComparison.Ordinal);
        Assert.Contains("\"responseType\":\"orderSummaryAnalytics\"", json, StringComparison.Ordinal);
        Assert.Contains("\"data\"", json, StringComparison.Ordinal);
        Assert.Contains("\"totalSpend\":12.50", json, StringComparison.Ordinal);
        Assert.Contains("\"orderCount\":2", json, StringComparison.Ordinal);
        Assert.DoesNotContain("ResponseType", json, StringComparison.Ordinal);
        Assert.DoesNotContain("DataScope", json, StringComparison.Ordinal);
    }

    [Fact]
    public void AssistantTypes_ShouldNotDependOnPersistenceRepositoriesDomainOrMcp()
    {
        var result = Types.InAssembly(typeof(AssistantOrchestrator).Assembly)
            .That()
            .ResideInNamespace("Ecommerce.Api.Assistant")
            .Should()
            .NotHaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Ecommerce.Catalog.Domain",
                "Ecommerce.Auth.Domain",
                "Ecommerce.Orders.Domain",
                "Ecommerce.Catalog.Infrastructure",
                "Ecommerce.Auth.Infrastructure",
                "Ecommerce.Orders.Infrastructure",
                "Ecommerce.Catalog.Application.Products.IProductRepository",
                "Ecommerce.Orders.Application.Orders.IOrderRepository",
                "ModelContextProtocol")
            .GetResult();

        Assert.True(result.IsSuccessful, "Assistant types must remain API orchestration and avoid persistence, Domain, repositories, and MCP protocol dependencies.");
    }

    [Fact]
    public void AssistantSource_ShouldNotReferenceWriteCommands()
    {
        var root = ProjectGraph.GetRootPath();
        var assistantFiles = Directory
            .EnumerateFiles(Path.Combine(root, "src", "Api", "Ecommerce.Api", "Assistant"), "*.cs", SearchOption.AllDirectories)
            .Concat(Directory.EnumerateFiles(Path.Combine(root, "src", "Api", "Ecommerce.Api", "Controllers", "Assistant"), "*.cs", SearchOption.AllDirectories));
        var source = string.Join(Environment.NewLine, assistantFiles.Select(File.ReadAllText));

        Assert.DoesNotContain("CreateOrderCommand", source, StringComparison.Ordinal);
        Assert.DoesNotContain("CreateProductCommand", source, StringComparison.Ordinal);
        Assert.DoesNotContain("UpdateProduct", source, StringComparison.Ordinal);
        Assert.DoesNotContain("DeactivateProduct", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ReactivateProduct", source, StringComparison.Ordinal);
    }

    public static IEnumerable<object[]> InvalidModelPlans()
    {
        yield return
        [
            new AssistantIntentPlan(
                AssistantIntentKind.RecentOrders,
                ["orders_delete"],
                AssistantIntentPlan.EmptyArguments())
        ];
        yield return
        [
            new AssistantIntentPlan(
                AssistantIntentKind.RecentOrders,
                [AssistantToolNames.OrdersSearch],
                new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["buyerId"] = Guid.NewGuid().ToString()
                })
        ];
        yield return
        [
            new AssistantIntentPlan(
                AssistantIntentKind.CatalogGetProduct,
                [AssistantToolNames.CatalogGetProduct],
                new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["productId"] = "not-a-guid"
                })
        ];
        yield return
        [
            new AssistantIntentPlan(
                AssistantIntentKind.CatalogProductsUnderPrice,
                [AssistantToolNames.CatalogSearch],
                new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["amount"] = "-1"
                })
        ];
    }

    private static AssistantOrchestrator CreateOrchestrator(
        ISender sender,
        IAssistantIntentInterpreter? interpreter = null)
    {
        var safetyPolicy = new AssistantSafetyPolicy();
        var intentRouter = new AssistantIntentRouter(safetyPolicy);
        var deterministicInterpreter = new DeterministicAssistantIntentInterpreter(intentRouter);
        var toolRegistry = new AssistantToolRegistry();

        return new AssistantOrchestrator(
            sender,
            interpreter ?? deterministicInterpreter,
            deterministicInterpreter,
            new AssistantIntentPlanValidator(toolRegistry, safetyPolicy),
            toolRegistry,
            NullLogger<AssistantOrchestrator>.Instance,
            Options.Create(new AssistantLlmOptions()));
    }

    private static LlmAssistantIntentInterpreter CreateLlmInterpreter(
        IAssistantLlmClient client,
        bool enabled = true)
    {
        var options = Options.Create(new AssistantLlmOptions
        {
            Enabled = enabled,
            Endpoint = "https://example.test/v1/responses",
            Model = "test-model",
            TimeoutSeconds = 5,
            MaxResponseCharacters = 4000
        });

        return new LlmAssistantIntentInterpreter(
            client,
            new AssistantIntentPlanJsonParser(),
            options,
            NullLogger<LlmAssistantIntentInterpreter>.Instance);
    }

    private static AssistantController CreateController(Guid? userId)
    {
        var controller = new AssistantController(CreateOrchestrator(new RecordingSender(_ => null)));
        var claims = userId.HasValue
            ? new[] { new Claim("sub", userId.Value.ToString()) }
            : Array.Empty<Claim>();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "Test"))
            }
        };

        return controller;
    }

    private static AssistantLlmOptions CreateEnabledLlmOptions() =>
        new()
        {
            Enabled = true,
            Endpoint = "https://example.test/v1/responses",
            Model = "test-model",
            ApiKey = "test-api-key",
            ApiKeyEnvironmentVariable = string.Empty,
            TimeoutSeconds = 5,
            MaxResponseCharacters = 4000
        };

    private sealed class RecordingSender(Func<object, object?> handler) : ISender
    {
        public List<object> Requests { get; } = [];

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return Task.FromResult((TResponse)handler(request)!);
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            Requests.Add(request);
            handler(request);
            return Task.CompletedTask;
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return Task.FromResult(handler(request));
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class StaticPlanInterpreter(AssistantIntentPlan plan) : IAssistantIntentInterpreter
    {
        public Task<AssistantIntentPlan?> InterpretAsync(
            string question,
            CancellationToken cancellationToken) =>
            Task.FromResult<AssistantIntentPlan?>(plan);
    }

    private sealed class ThrowingPlanInterpreter : IAssistantIntentInterpreter
    {
        public Task<AssistantIntentPlan?> InterpretAsync(
            string question,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException("Interpreter unavailable.");
    }

    private sealed class RecordingLlmClient(string? response) : IAssistantLlmClient
    {
        public List<string> Questions { get; } = [];

        public Task<string?> CreateIntentPlanJsonAsync(
            string question,
            CancellationToken cancellationToken)
        {
            Questions.Add(question);
            return Task.FromResult(response);
        }
    }

    private sealed class ThrowingLlmClient : IAssistantLlmClient
    {
        public Task<string?> CreateIntentPlanJsonAsync(
            string question,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException("Provider unavailable.");
    }

    private sealed class CanceledLlmClient : IAssistantLlmClient
    {
        public Task<string?> CreateIntentPlanJsonAsync(
            string question,
            CancellationToken cancellationToken) =>
            throw new OperationCanceledException();
    }

    private sealed class CountingHttpMessageHandler : HttpMessageHandler
    {
        public int SendCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            SendCount++;
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        }
    }

    private sealed class RecordingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) : HttpMessageHandler
    {
        public string? RequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return handler(request);
        }
    }

    private sealed class ListLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull =>
            null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
