using System.Net;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Ecommerce.Api.Assistant;
using Ecommerce.Api.Assistant.TextToSql;
using Ecommerce.Api.Controllers.Assistant;
using CatalogPagedResult = Ecommerce.Catalog.Application.Abstractions.PagedResult<Ecommerce.Catalog.Application.Products.SearchProducts.ProductListItemDto>;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NetArchTest.Rules;
using OrdersPagedResult = Ecommerce.Orders.Application.Abstractions.PagedResult<Ecommerce.Orders.Application.Orders.ListOrdersForBuyer.OrderSummaryDto>;

namespace Ecommerce.ArchitectureTests;

public sealed class AssistantIntegrationTests : IDisposable
{
    private readonly ScopedAssistantEnvironment assistantEnvironment = ScopedAssistantEnvironment.Clear();

    public void Dispose() => assistantEnvironment.Dispose();

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
        Assert.Contains("AddHttpClient<HttpAssistantLlmClient>", source, StringComparison.Ordinal);
        Assert.Contains("AddHttpClient<GeminiAssistantLlmClient>", source, StringComparison.Ordinal);
        Assert.Contains("IsGeminiProvider", source, StringComparison.Ordinal);
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

    [Fact]
    public async Task GeminiLlmClient_MissingSecret_ShouldReturnNoPlanWithoutSendingRequest()
    {
        var missingSecretName = "ECOMMERCE_ASSISTANT_TEST_MISSING_GEMINI_SECRET_" + Guid.NewGuid().ToString("N");
        var handler = new CountingHttpMessageHandler();
        var client = new GeminiAssistantLlmClient(
            new HttpClient(handler),
            Options.Create(new AssistantLlmOptions
            {
                Enabled = true,
                Provider = AssistantLlmOptions.GeminiProvider,
                GeminiEndpoint = "https://example.test/v1beta",
                GeminiModel = "gemini-test",
                GeminiApiKeyEnvironmentVariable = missingSecretName
            }),
            NullLogger<GeminiAssistantLlmClient>.Instance);

        var planJson = await client.CreateIntentPlanJsonAsync(
            "Show my recent orders",
            CancellationToken.None);

        Assert.Null(planJson);
        Assert.Equal(0, handler.SendCount);
    }

    [Fact]
    public async Task GeminiLlmClient_ShouldSendGenerateContentRequestShapeAndParseCandidateText()
    {
        const string question = "Could you recap my latest shopping activity?";
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {"candidates":[{"content":{"parts":[{"text":"{\"kind\":\"RecentOrders\",\"tools\":[\"orders_search\"],\"arguments\":{}}"}]}}]}
                """,
                Encoding.UTF8,
                "application/json")
        });
        var client = new GeminiAssistantLlmClient(
            new HttpClient(handler),
            Options.Create(CreateEnabledGeminiOptions()),
            NullLogger<GeminiAssistantLlmClient>.Instance);

        var planJson = await client.CreateIntentPlanJsonAsync(
            question,
            CancellationToken.None);

        Assert.Equal("""{"kind":"RecentOrders","tools":["orders_search"],"arguments":{}}""", planJson);
        Assert.Equal(HttpMethod.Post, handler.RequestMethod);
        Assert.NotNull(handler.RequestUri);
        Assert.Equal(
            "https://example.test/v1beta/models/gemini-test:generateContent?key=test-gemini-key",
            handler.RequestUri!.AbsoluteUri);
        Assert.Null(handler.AuthorizationHeader);
        Assert.NotNull(handler.RequestBody);

        using var document = JsonDocument.Parse(handler.RequestBody);
        var systemText = document.RootElement
            .GetProperty("systemInstruction")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();
        var userText = document.RootElement
            .GetProperty("contents")[0]
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        Assert.Contains("read-only ecommerce backend assistant", systemText, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(question, userText);
        Assert.Equal(0, document.RootElement.GetProperty("generationConfig").GetProperty("temperature").GetInt32());
        Assert.Equal("application/json", document.RootElement.GetProperty("generationConfig").GetProperty("responseMimeType").GetString());
        Assert.DoesNotContain("orders", userText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("catalog", userText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GeminiLlmClient_MalformedProviderJson_ShouldReturnNullSafely()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("not json", Encoding.UTF8, "application/json")
        });
        var client = new GeminiAssistantLlmClient(
            new HttpClient(handler),
            Options.Create(CreateEnabledGeminiOptions()),
            NullLogger<GeminiAssistantLlmClient>.Instance);

        var planJson = await client.CreateIntentPlanJsonAsync(
            "Show my recent orders",
            CancellationToken.None);

        Assert.Null(planJson);
    }

    [Fact]
    public async Task GeminiLlmClient_MissingCandidateText_ShouldReturnNullSafely()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {"candidates":[{"content":{"parts":[{}]}}]}
                """,
                Encoding.UTF8,
                "application/json")
        });
        var client = new GeminiAssistantLlmClient(
            new HttpClient(handler),
            Options.Create(CreateEnabledGeminiOptions()),
            NullLogger<GeminiAssistantLlmClient>.Instance);

        var planJson = await client.CreateIntentPlanJsonAsync(
            "Show my recent orders",
            CancellationToken.None);

        Assert.Null(planJson);
    }

    [Fact]
    public async Task GeminiLlmClient_BadRequest_ShouldLogOnlySafeProviderDiagnostics()
    {
        const string apiKey = "test-gemini-key";
        const string promptText = "Show my recent orders with GEMINI_PROMPT_SHOULD_NOT_LOG";
        var logger = new ListLogger<GeminiAssistantLlmClient>();
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent(
                """
                {"error":{"code":429,"status":"RESOURCE_EXHAUSTED","message":"Quota exceeded for 'GEMINI_PROMPT_SHOULD_NOT_LOG'."}}
                """,
                Encoding.UTF8,
                "application/json")
        });
        var client = new GeminiAssistantLlmClient(
            new HttpClient(handler),
            Options.Create(CreateEnabledGeminiOptions()),
            logger);

        var planJson = await client.CreateIntentPlanJsonAsync(
            promptText,
            CancellationToken.None);

        Assert.Null(planJson);

        var log = Assert.Single(logger.Messages);
        Assert.Contains("statusCode=429", log, StringComparison.Ordinal);
        Assert.Contains("geminiErrorCode=429", log, StringComparison.Ordinal);
        Assert.Contains("geminiErrorStatus=RESOURCE_EXHAUSTED", log, StringComparison.Ordinal);
        Assert.Contains("sanitizedErrorMessage=Quota exceeded for [redacted].", log, StringComparison.Ordinal);
        Assert.DoesNotContain(apiKey, log, StringComparison.Ordinal);
        Assert.DoesNotContain("Authorization", log, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Bearer", log, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(promptText, log, StringComparison.Ordinal);
        Assert.DoesNotContain("GEMINI_PROMPT_SHOULD_NOT_LOG", log, StringComparison.Ordinal);
    }

    [Fact]
    public void ProviderSelection_ShouldChooseGeminiClientWhenConfigured()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IOptions<AssistantLlmOptions>>(Options.Create(CreateEnabledGeminiOptions()));
        services.AddHttpClient<HttpAssistantLlmClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new CountingHttpMessageHandler());
        services.AddHttpClient<GeminiAssistantLlmClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new CountingHttpMessageHandler());
        services.AddScoped<IAssistantLlmClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<AssistantLlmOptions>>().Value;
            return options.IsGeminiProvider
                ? provider.GetRequiredService<GeminiAssistantLlmClient>()
                : provider.GetRequiredService<HttpAssistantLlmClient>();
        });

        using var provider = services.BuildServiceProvider();

        Assert.IsType<GeminiAssistantLlmClient>(provider.GetRequiredService<IAssistantLlmClient>());
    }

    [Fact]
    public void Program_ShouldRegisterFeatureFlaggedTextToSqlServices()
    {
        var root = ProjectGraph.GetRootPath();
        var program = File.ReadAllText(Path.Combine(root, "src", "Api", "Ecommerce.Api", "Program.cs"));
        var appsettings = File.ReadAllText(Path.Combine(root, "src", "Api", "Ecommerce.Api", "appsettings.json"));

        Assert.Contains("Configure<AssistantTextToSqlOptions>", program, StringComparison.Ordinal);
        Assert.Contains("AssistantSqlValidator", program, StringComparison.Ordinal);
        Assert.Contains("IAssistantReadOnlySqlExecutor", program, StringComparison.Ordinal);
        Assert.Contains("IAssistantTextToSqlPlanner", program, StringComparison.Ordinal);
        Assert.Contains("AssistantTextToSqlPromptBuilder", program, StringComparison.Ordinal);
        Assert.Contains("AssistantTextToSqlPlanParser", program, StringComparison.Ordinal);
        Assert.Contains("AssistantTextToSqlResponseMapper", program, StringComparison.Ordinal);
        Assert.DoesNotContain("AssistantOrchestrator", program[program.IndexOf("IAssistantReadOnlySqlExecutor", StringComparison.Ordinal)..], StringComparison.Ordinal);
        Assert.Contains("\"TextToSql\"", appsettings, StringComparison.Ordinal);
        Assert.Contains("\"Enabled\": false", appsettings, StringComparison.Ordinal);
        Assert.DoesNotContain("AssistantCatalogReadOnly", appsettings, StringComparison.Ordinal);
        Assert.DoesNotContain("AssistantOrdersReadOnly", appsettings, StringComparison.Ordinal);
    }

    [Fact]
    public void Program_ShouldRegisterOrdersAssistantSubAgent()
    {
        var root = ProjectGraph.GetRootPath();
        var program = File.ReadAllText(Path.Combine(root, "src", "Api", "Ecommerce.Api", "Program.cs"));

        Assert.Contains("AddScoped<IOrdersAssistantSubAgent, OrdersAssistantSubAgent>", program, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) ProductId, Name FROM assistant.v_ProductSearch")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (5) ProductId, Sku FROM assistant.v_ProductDetails WHERE IsActive = 1")]
    [InlineData(AssistantSqlDataSource.Orders, "SELECT TOP (10) OrderId, Status FROM assistant.v_MyOrders WHERE BuyerUserId = @CurrentUserId")]
    [InlineData(AssistantSqlDataSource.Orders, "SELECT TOP (10) ProductName FROM assistant.v_MyOrderLines WHERE BuyerUserId = @CurrentUserId")]
    [InlineData(AssistantSqlDataSource.Orders, "SELECT TOP (1) o.OrderId, o.Status, o.TotalAmount, o.CreatedAt, o.LineCount, l.ProductId, l.ProductName, l.ProductSku, l.Quantity, l.UnitPriceAmount, l.LineTotal FROM assistant.v_MyOrders AS o INNER JOIN assistant.v_MyOrderLines AS l ON l.OrderId = o.OrderId WHERE o.BuyerUserId = @CurrentUserId AND l.BuyerUserId = @CurrentUserId AND (l.ProductName LIKE '%Galaxy%' OR l.ProductSku LIKE '%Galaxy%') ORDER BY o.CreatedAt ASC")]
    public void SqlValidator_ShouldAcceptApprovedAssistantViewQueries(
        AssistantSqlDataSource dataSource,
        string sql)
    {
        var result = CreateSqlValidator(maxRows: 50).Validate(new AssistantSqlQuery(dataSource, sql));

        Assert.True(result.IsValid, result.Reason);
    }

    [Theory]
    [InlineData(AssistantSqlDataSource.Orders, "SELECT TOP (10) OrderId FROM assistant.v_MyOrders")]
    [InlineData(AssistantSqlDataSource.Orders, "SELECT TOP (10) OrderId FROM assistant.v_MyOrders WHERE BuyerUserId = '00000000-0000-0000-0000-000000000001'")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) Id FROM catalog.Products")]
    [InlineData(AssistantSqlDataSource.Orders, "SELECT TOP (10) Id FROM orders.Orders WHERE BuyerUserId = @CurrentUserId")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) Id FROM dbo.Products")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) Id FROM auth.Users")]
    [InlineData(AssistantSqlDataSource.Catalog, "INSERT INTO assistant.v_ProductSearch (Name) VALUES ('x')")]
    [InlineData(AssistantSqlDataSource.Catalog, "UPDATE assistant.v_ProductSearch SET Name = 'x'")]
    [InlineData(AssistantSqlDataSource.Catalog, "DELETE FROM assistant.v_ProductSearch")]
    [InlineData(AssistantSqlDataSource.Catalog, "MERGE assistant.v_ProductSearch AS target USING assistant.v_ProductDetails AS source ON 1 = 1")]
    [InlineData(AssistantSqlDataSource.Catalog, "CREATE VIEW assistant.bad AS SELECT 1")]
    [InlineData(AssistantSqlDataSource.Catalog, "ALTER VIEW assistant.v_ProductSearch AS SELECT 1")]
    [InlineData(AssistantSqlDataSource.Catalog, "DROP VIEW assistant.v_ProductSearch")]
    [InlineData(AssistantSqlDataSource.Catalog, "TRUNCATE TABLE assistant.v_ProductSearch")]
    [InlineData(AssistantSqlDataSource.Catalog, "EXEC dbo.SomeProcedure")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) ProductId FROM assistant.v_ProductSearch; SELECT TOP (10) ProductId FROM assistant.v_ProductDetails")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) ProductId FROM assistant.v_ProductSearch -- nope")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) ProductId FROM assistant.v_ProductSearch /* nope */")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) name FROM sys.objects")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) table_name FROM INFORMATION_SCHEMA.TABLES")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) ProductId FROM assistant.v_Unsafe")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (10) OrderId FROM assistant.v_MyOrders")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT ProductId FROM assistant.v_ProductSearch")]
    [InlineData(AssistantSqlDataSource.Catalog, "SELECT TOP (51) ProductId FROM assistant.v_ProductSearch")]
    public void SqlValidator_ShouldRejectUnsafeOrUnapprovedQueries(
        AssistantSqlDataSource dataSource,
        string sql)
    {
        var result = CreateSqlValidator(maxRows: 50).Validate(new AssistantSqlQuery(dataSource, sql));

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(AssistantSqlDataSource.Catalog, "Server=.;Database=CatalogReadOnly;Trusted_Connection=True;TrustServerCertificate=True")]
    [InlineData(AssistantSqlDataSource.Orders, "Server=.;Database=OrdersReadOnly;Trusted_Connection=True;TrustServerCertificate=True")]
    public void SqlConnectionFactory_ShouldChooseReadOnlyConnectionForDataSource(
        AssistantSqlDataSource dataSource,
        string expectedConnectionString)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:AssistantCatalogReadOnly"] = "Server=.;Database=CatalogReadOnly;Trusted_Connection=True;TrustServerCertificate=True",
                ["ConnectionStrings:AssistantOrdersReadOnly"] = "Server=.;Database=OrdersReadOnly;Trusted_Connection=True;TrustServerCertificate=True"
            })
            .Build();

        using var connection = new AssistantSqlConnectionFactory(configuration).CreateConnection(dataSource);

        Assert.Equal(expectedConnectionString, connection.ConnectionString);
    }

    [Fact]
    public void SqlConnectionFactory_ShouldRequireConfiguredConnectionString()
    {
        var configuration = new ConfigurationBuilder().Build();

        Assert.Throws<InvalidOperationException>(() =>
            new AssistantSqlConnectionFactory(configuration).CreateConnection(AssistantSqlDataSource.Catalog));
    }

    [Fact]
    public async Task SqlExecutor_ShouldApplyTimeoutMaxRowsAndCurrentUserParameter()
    {
        var currentUserId = Guid.NewGuid();
        var command = new RecordingDbCommand(new FakeDbDataReader(
            ["OrderId"],
            [
                [Guid.NewGuid()],
                [Guid.NewGuid()]
            ]));
        var executor = CreateSqlExecutor(new RecordingDbConnection(command), maxRows: 1, timeoutSeconds: 7);

        var result = await executor.ExecuteAsync(
            new AssistantSqlQuery(
                AssistantSqlDataSource.Orders,
                "SELECT TOP (1) OrderId FROM assistant.v_MyOrders WHERE BuyerUserId = @CurrentUserId",
                currentUserId),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.True(result.Truncated);
        Assert.Equal(1, result.RowCount);
        Assert.Equal(7, command.CommandTimeout);
        var parameter = Assert.Single(command.Parameters.Cast<DbParameter>());
        Assert.Equal("@CurrentUserId", parameter.ParameterName);
        Assert.Equal(currentUserId, parameter.Value);
    }

    [Fact]
    public async Task SqlExecutor_ShouldNotExecuteWhenFeatureFlagDisabled()
    {
        var command = new RecordingDbCommand(new FakeDbDataReader(["ProductId"], [[Guid.NewGuid()]]));
        var executor = new AssistantReadOnlySqlExecutor(
            CreateSqlValidator(),
            new StubSqlConnectionFactory(new RecordingDbConnection(command)),
            Options.Create(new AssistantTextToSqlOptions
            {
                Enabled = false,
                MaxRows = 50,
                CommandTimeoutSeconds = 5
            }),
            NullLogger<AssistantReadOnlySqlExecutor>.Instance);

        var result = await executor.ExecuteAsync(
            new AssistantSqlQuery(
                AssistantSqlDataSource.Catalog,
                "SELECT TOP (1) ProductId FROM assistant.v_ProductSearch"),
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ConnectionState.Closed, command.Connection?.State ?? ConnectionState.Closed);
    }

    [Fact]
    public async Task SqlExecutor_ShouldReturnGenericFailureWithoutRawExceptionText()
    {
        var command = new RecordingDbCommand(new InvalidOperationException("raw sql failure text"));
        var executor = CreateSqlExecutor(new RecordingDbConnection(command));

        var result = await executor.ExecuteAsync(
            new AssistantSqlQuery(
                AssistantSqlDataSource.Catalog,
                "SELECT TOP (1) ProductId FROM assistant.v_ProductSearch"),
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("SQL execution failed.", result.Error);
        Assert.DoesNotContain("raw sql failure text", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TextToSqlPrompt_ShouldDescribeOnlyApprovedAssistantViewsAndRules()
    {
        var prompt = new AssistantTextToSqlPromptBuilder().BuildPrompt("what is my last order");

        Assert.Contains("assistant.v_ProductSearch", prompt, StringComparison.Ordinal);
        Assert.Contains("assistant.v_ProductDetails", prompt, StringComparison.Ordinal);
        Assert.Contains("assistant.v_MyOrders", prompt, StringComparison.Ordinal);
        Assert.Contains("assistant.v_MyOrderLines", prompt, StringComparison.Ordinal);
        Assert.Contains("assistant.v_MyOrderSummary", prompt, StringComparison.Ordinal);
        Assert.Contains("BuyerUserId = @CurrentUserId", prompt, StringComparison.Ordinal);
        Assert.Contains("deactivate product", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("first order where I ordered Galaxy", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("earliest order containing product X", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("show my orders where I bought Galaxy", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("l.ProductName LIKE '%Galaxy%'", prompt, StringComparison.Ordinal);
        Assert.Contains("l.ProductSku LIKE '%Galaxy%'", prompt, StringComparison.Ordinal);
        Assert.Contains("\"resultShape\":\"orderList\"", prompt, StringComparison.Ordinal);
        Assert.Contains("\"supported\":false", prompt, StringComparison.Ordinal);
        Assert.Contains("ProductId, Name, Sku, Description, PriceAmount, IsActive, CreatedAt, UpdatedAt", prompt, StringComparison.Ordinal);
        Assert.DoesNotContain("CurrencyCode", prompt, StringComparison.Ordinal);
        Assert.DoesNotContain("auth.Users", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("catalog.Products", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("orders.Orders", prompt, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TextToSqlPlanParser_ShouldAcceptSupportedPlan()
    {
        var plan = new AssistantTextToSqlPlanParser().Parse(
            """
            {"supported":true,"dataSource":"orders","sql":"SELECT TOP (1) OrderId FROM assistant.v_MyOrders WHERE BuyerUserId = @CurrentUserId","resultShape":"orderList","reason":null}
            """,
            4000);

        Assert.True(plan.Supported);
        Assert.Equal(AssistantSqlDataSource.Orders, plan.DataSource);
        Assert.Equal(AssistantTextToSqlResultShape.OrderList, plan.ResultShape);
        Assert.NotNull(plan.Sql);
    }

    [Fact]
    public void TextToSqlPlanParser_ShouldAcceptUnsupportedPlan()
    {
        var plan = new AssistantTextToSqlPlanParser().Parse(
            """
            {"supported":false,"dataSource":null,"sql":null,"resultShape":"unsupported","reason":"Write or admin operations are not supported."}
            """,
            4000);

        Assert.False(plan.Supported);
        Assert.Null(plan.DataSource);
        Assert.Null(plan.Sql);
        Assert.Equal(AssistantTextToSqlResultShape.Unsupported, plan.ResultShape);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("""{"supported":true,"dataSource":"orders","resultShape":"orderList"}""")]
    [InlineData("""{"supported":false,"dataSource":null,"sql":"SELECT TOP (1) ProductId FROM assistant.v_ProductSearch","resultShape":"unsupported"}""")]
    [InlineData("""{"supported":true,"dataSource":"inventory","sql":"SELECT TOP (1) ProductId FROM assistant.v_ProductSearch","resultShape":"productList"}""")]
    [InlineData("""{"supported":true,"dataSource":"catalog","sql":"SELECT TOP (1) ProductId FROM assistant.v_ProductSearch","resultShape":"chart"}""")]
    public void TextToSqlPlanParser_ShouldFailClosedForInvalidOutput(string json)
    {
        var plan = new AssistantTextToSqlPlanParser().Parse(json, 4000);

        Assert.False(plan.Supported);
        Assert.Null(plan.DataSource);
        Assert.Null(plan.Sql);
        Assert.Equal(AssistantTextToSqlResultShape.Unsupported, plan.ResultShape);
    }

    [Theory]
    [InlineData(
        "what is my last order",
        """{"supported":true,"dataSource":"orders","sql":"SELECT TOP (1) OrderId, Status, TotalAmount, CreatedAt, LineCount FROM assistant.v_MyOrders WHERE BuyerUserId = @CurrentUserId ORDER BY CreatedAt DESC","resultShape":"orderList","reason":null}""",
        AssistantSqlDataSource.Orders,
        AssistantTextToSqlResultShape.OrderList)]
    [InlineData(
        "find products under 20",
        """{"supported":true,"dataSource":"catalog","sql":"SELECT TOP (10) ProductId, Name, Sku, Description, PriceAmount, IsActive FROM assistant.v_ProductSearch WHERE IsActive = 1 AND PriceAmount < 20 ORDER BY PriceAmount ASC","resultShape":"productList","reason":null}""",
        AssistantSqlDataSource.Catalog,
        AssistantTextToSqlResultShape.ProductList)]
    public async Task LlmTextToSqlPlanner_ShouldParseProviderPlanAndPassSqlValidator(
        string question,
        string providerJson,
        AssistantSqlDataSource expectedDataSource,
        AssistantTextToSqlResultShape expectedShape)
    {
        var client = new RecordingLlmClient(providerJson);
        var planner = CreateTextToSqlPlanner(client, enabled: true);

        var plan = await planner.PlanAsync(question, CancellationToken.None);

        Assert.True(plan.Supported);
        Assert.Equal(expectedDataSource, plan.DataSource);
        Assert.Equal(expectedShape, plan.ResultShape);
        Assert.NotNull(plan.Sql);
        Assert.Single(client.Questions);
        Assert.Contains(question, client.Questions[0], StringComparison.Ordinal);
        Assert.DoesNotContain("CurrencyCode", client.Questions[0], StringComparison.Ordinal);

        var validation = CreateSqlValidator().Validate(new AssistantSqlQuery(plan.DataSource!.Value, plan.Sql!));
        Assert.True(validation.IsValid, validation.Reason);
    }

    [Fact]
    public async Task LlmTextToSqlPlanner_ShouldReturnUnsupportedWhenDisabledWithoutCallingProvider()
    {
        var client = new RecordingLlmClient(
            """{"supported":true,"dataSource":"catalog","sql":"SELECT TOP (1) ProductId FROM assistant.v_ProductSearch","resultShape":"productList","reason":null}""");
        var planner = CreateTextToSqlPlanner(client, enabled: false);

        var plan = await planner.PlanAsync("find products", CancellationToken.None);

        Assert.False(plan.Supported);
        Assert.Empty(client.Questions);
    }

    [Fact]
    public async Task TextToSqlDisabled_ShouldKeepExistingAssistantPathAndNotCallPlannerOrExecutor()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(
                new[] { new OrderSummaryDto(Guid.NewGuid(), "Created", 42.50m, DateTimeOffset.UtcNow, 2) },
                query.PageNumber ?? 1,
                query.PageSize ?? 100,
                1),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var planner = new RecordingTextToSqlPlanner(OrdersPlan());
        var executor = new RecordingTextToSqlExecutor(OrderListResult());
        var orchestrator = CreateOrchestrator(
            sender,
            textToSqlPlanner: planner,
            textToSqlExecutor: executor,
            textToSqlEnabled: false);

        var response = await orchestrator.QueryAsync("Show my recent orders", buyerId, CancellationToken.None);

        Assert.Empty(planner.Questions);
        Assert.Empty(executor.Queries);
        Assert.Single(sender.Requests);
        Assert.False(response.Unsupported);
        Assert.Equal(AssistantResponseTypes.RecentOrders, response.ResponseType);
    }

    [Fact]
    public async Task TextToSqlEnabled_LastOrder_ShouldUseOrdersPlanAndCurrentUserScope()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(_ => throw new InvalidOperationException("Existing assistant path should not be called."));
        var planner = new RecordingTextToSqlPlanner(OrdersPlan(top: 1));
        var executor = new RecordingTextToSqlExecutor(OrderListResult());
        var orchestrator = CreateOrchestrator(
            sender,
            textToSqlPlanner: planner,
            textToSqlExecutor: executor,
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("what is my last order", buyerId, CancellationToken.None);

        var query = Assert.Single(executor.Queries);
        Assert.Equal(AssistantSqlDataSource.Orders, query.DataSource);
        Assert.Equal(buyerId, query.CurrentUserId);
        Assert.Contains("@CurrentUserId", query.Sql, StringComparison.Ordinal);
        Assert.Contains("TOP (1)", query.Sql, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(sender.Requests);
        Assert.False(response.Unsupported);
        Assert.Equal(AssistantResponseTypes.RecentOrders, response.ResponseType);
        Assert.DoesNotContain("SELECT", response.Answer, StringComparison.OrdinalIgnoreCase);

        var data = Assert.IsType<AssistantOrdersData>(response.Data);
        Assert.Single(data.Orders);
    }

    [Fact]
    public async Task TextToSqlEnabled_RecentOrders_ShouldMapAssistantOrdersData()
    {
        var planner = new RecordingTextToSqlPlanner(OrdersPlan());
        var executor = new RecordingTextToSqlExecutor(OrderListResult());
        var orchestrator = CreateOrchestrator(
            new RecordingSender(_ => throw new InvalidOperationException("Existing assistant path should not be called.")),
            textToSqlPlanner: planner,
            textToSqlExecutor: executor,
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("show my recent orders", Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(AssistantResponseTypes.RecentOrders, response.ResponseType);
        Assert.Contains(AssistantToolNames.OrdersSearch, response.ToolsUsed);
        Assert.Equal("authenticated-user", response.DataScope);
        Assert.IsType<AssistantOrdersData>(response.Data);
    }

    [Fact]
    public async Task TextToSqlEnabled_OrderProductMatch_ShouldMapRecentOrdersWithMatchingLines()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(_ => throw new InvalidOperationException("Existing assistant path should not be called."));
        var plan = OrderProductMatchPlan();
        var planner = new RecordingTextToSqlPlanner(plan);
        var executor = new RecordingTextToSqlExecutor(OrderProductMatchResult());
        var orchestrator = CreateOrchestrator(
            sender,
            textToSqlPlanner: planner,
            textToSqlExecutor: executor,
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync(
            "I need my first order where I order a Galaxy product",
            buyerId,
            CancellationToken.None);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var query = Assert.Single(executor.Queries);
        Assert.Equal(AssistantSqlDataSource.Orders, query.DataSource);
        Assert.Equal(buyerId, query.CurrentUserId);
        Assert.Empty(sender.Requests);
        Assert.False(response.Unsupported);
        Assert.Equal(AssistantResponseTypes.RecentOrders, response.ResponseType);
        Assert.Contains(AssistantToolNames.OrdersSearch, response.ToolsUsed);
        Assert.DoesNotContain(plan.Sql!, json, StringComparison.Ordinal);
        Assert.DoesNotContain("SELECT", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("assistant.v_MyOrders", json, StringComparison.OrdinalIgnoreCase);

        var data = Assert.IsType<AssistantOrdersData>(response.Data);
        var order = Assert.Single(data.Orders);
        Assert.Equal("Created", order.Status);
        Assert.Equal(2, order.LineCount);
        var line = Assert.Single(order.Lines);
        Assert.Equal("Galaxy Buds", line.ProductName);
        Assert.Equal("GALAXY-BUDS", line.ProductSku);
        Assert.Equal(1, line.Quantity);
    }

    [Fact]
    public async Task TextToSqlEnabled_OrderProductMatch_ShouldGroupMatchingLinesByOrder()
    {
        var orchestrator = CreateOrchestrator(
            new RecordingSender(_ => throw new InvalidOperationException("Existing assistant path should not be called.")),
            textToSqlPlanner: new RecordingTextToSqlPlanner(OrderProductMatchPlan()),
            textToSqlExecutor: new RecordingTextToSqlExecutor(OrderProductMatchResult(matchingLineRows: 2)),
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync(
            "show my orders where I bought Galaxy",
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(response.Unsupported);
        Assert.Equal(AssistantResponseTypes.RecentOrders, response.ResponseType);

        var data = Assert.IsType<AssistantOrdersData>(response.Data);
        var order = Assert.Single(data.Orders);
        Assert.Equal(2, order.Lines.Count);
        Assert.Contains(order.Lines, line => line.ProductSku == "GALAXY-BUDS");
        Assert.Contains(order.Lines, line => line.ProductSku == "GALAXY-CASE");
    }

    [Fact]
    public async Task TextToSqlEnabled_EmptyOrderProductMatch_ShouldReturnSupportedEmptyState()
    {
        var orchestrator = CreateOrchestrator(
            new RecordingSender(_ => throw new InvalidOperationException("Existing assistant path should not be called.")),
            textToSqlPlanner: new RecordingTextToSqlPlanner(OrderProductMatchPlan()),
            textToSqlExecutor: new RecordingTextToSqlExecutor(new AssistantSqlResult(
                true,
                [
                    "OrderId",
                    "Status",
                    "TotalAmount",
                    "CreatedAt",
                    "LineCount",
                    "ProductId",
                    "ProductName",
                    "ProductSku",
                    "Quantity",
                    "UnitPriceAmount",
                    "LineTotal"
                ],
                Array.Empty<AssistantSqlRow>(),
                0,
                false)),
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync(
            "I need my first order where I order a Galaxy product",
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(response.Unsupported);
        Assert.Equal(AssistantResponseTypes.RecentOrders, response.ResponseType);
        Assert.Contains("did not find matching orders", response.Answer, StringComparison.OrdinalIgnoreCase);

        var data = Assert.IsType<AssistantOrdersData>(response.Data);
        Assert.Empty(data.Orders);
    }

    [Fact]
    public async Task TextToSqlEnabled_TotalSpend_ShouldMapSpendSummary()
    {
        var planner = new RecordingTextToSqlPlanner(new AssistantTextToSqlPlan(
            true,
            AssistantSqlDataSource.Orders,
            "SELECT TOP (1) TotalOrders, TotalSpend, LastOrderDate FROM assistant.v_MyOrderSummary WHERE BuyerUserId = @CurrentUserId",
            AssistantTextToSqlResultShape.SpendSummary,
            null));
        var executor = new RecordingTextToSqlExecutor(new AssistantSqlResult(
            true,
            ["TotalOrders", "TotalSpend", "LastOrderDate"],
            [new AssistantSqlRow(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["TotalOrders"] = 2,
                ["TotalSpend"] = 50.50m,
                ["LastOrderDate"] = DateTimeOffset.UtcNow
            })],
            1,
            false));
        var orchestrator = CreateOrchestrator(
            new RecordingSender(_ => throw new InvalidOperationException("Existing assistant path should not be called.")),
            textToSqlPlanner: planner,
            textToSqlExecutor: executor,
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("what is my total spend", Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(AssistantResponseTypes.OrderSummaryAnalytics, response.ResponseType);
        Assert.Contains(AssistantToolNames.OrdersAnalyze, response.ToolsUsed);

        var data = Assert.IsType<AssistantOrderSummaryAnalyticsData>(response.Data);
        Assert.Equal(50.50m, data.TotalSpend);
        Assert.Equal(2, data.OrderCount);
    }

    [Fact]
    public async Task TextToSqlEnabled_ProductList_ShouldUseCatalogDataSourceAndMapProducts()
    {
        var productId = Guid.NewGuid();
        var planner = new RecordingTextToSqlPlanner(new AssistantTextToSqlPlan(
            true,
            AssistantSqlDataSource.Catalog,
            "SELECT TOP (10) ProductId, Name, Sku, Description, PriceAmount, IsActive FROM assistant.v_ProductSearch WHERE IsActive = 1 AND PriceAmount < 20 ORDER BY PriceAmount ASC",
            AssistantTextToSqlResultShape.ProductList,
            null));
        var executor = new RecordingTextToSqlExecutor(new AssistantSqlResult(
            true,
            ["ProductId", "Name", "Sku", "Description", "PriceAmount", "IsActive"],
            [new AssistantSqlRow(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["ProductId"] = productId,
                ["Name"] = "Tea",
                ["Sku"] = "SKU-1",
                ["Description"] = "Green tea",
                ["PriceAmount"] = 9.99m,
                ["IsActive"] = true
            })],
            1,
            false));
        var orchestrator = CreateOrchestrator(
            new RecordingSender(_ => throw new InvalidOperationException("Existing assistant path should not be called.")),
            textToSqlPlanner: planner,
            textToSqlExecutor: executor,
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("find products under 20", Guid.NewGuid(), CancellationToken.None);

        var query = Assert.Single(executor.Queries);
        Assert.Equal(AssistantSqlDataSource.Catalog, query.DataSource);
        Assert.Null(query.CurrentUserId);
        Assert.Equal("catalog-public", response.DataScope);
        Assert.Equal(AssistantResponseTypes.CatalogProducts, response.ResponseType);
        Assert.Contains(AssistantToolNames.CatalogSearch, response.ToolsUsed);

        var data = Assert.IsType<AssistantCatalogProductsData>(response.Data);
        var product = Assert.Single(data.Products);
        Assert.Equal(productId, product.ProductId);
        Assert.Equal(9.99m, product.Price);
    }

    [Fact]
    public async Task TextToSqlEnabled_PlannerUnsupported_ShouldFallBackSafely()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(Array.Empty<OrderSummaryDto>(), query.PageNumber ?? 1, query.PageSize ?? 100, 0),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var planner = new RecordingTextToSqlPlanner(AssistantTextToSqlPlan.Unsupported());
        var executor = new RecordingTextToSqlExecutor(OrderListResult());
        var orchestrator = CreateOrchestrator(
            sender,
            textToSqlPlanner: planner,
            textToSqlExecutor: executor,
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("show my recent orders", buyerId, CancellationToken.None);

        Assert.Single(planner.Questions);
        Assert.Empty(executor.Queries);
        Assert.Single(sender.Requests);
        Assert.False(response.Unsupported);
    }

    [Fact]
    public async Task TextToSqlEnabled_ValidatorFailure_ShouldFallBackSafely()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(Array.Empty<OrderSummaryDto>(), query.PageNumber ?? 1, query.PageSize ?? 100, 0),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var planner = new RecordingTextToSqlPlanner(new AssistantTextToSqlPlan(
            true,
            AssistantSqlDataSource.Orders,
            "SELECT TOP (1) OrderId FROM assistant.v_MyOrders",
            AssistantTextToSqlResultShape.OrderList,
            null));
        var executor = new RecordingTextToSqlExecutor(OrderListResult());
        var orchestrator = CreateOrchestrator(
            sender,
            textToSqlPlanner: planner,
            textToSqlExecutor: executor,
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("show my recent orders", buyerId, CancellationToken.None);

        Assert.Empty(executor.Queries);
        Assert.Single(sender.Requests);
        Assert.False(response.Unsupported);
    }

    [Fact]
    public async Task TextToSqlEnabled_ExecutorFailure_ShouldFallBackSafely()
    {
        var buyerId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(Array.Empty<OrderSummaryDto>(), query.PageNumber ?? 1, query.PageSize ?? 100, 0),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var executor = new RecordingTextToSqlExecutor(AssistantSqlResult.Failure());
        var orchestrator = CreateOrchestrator(
            sender,
            textToSqlPlanner: new RecordingTextToSqlPlanner(OrdersPlan()),
            textToSqlExecutor: executor,
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("show my recent orders", buyerId, CancellationToken.None);

        Assert.Single(executor.Queries);
        Assert.Single(sender.Requests);
        Assert.False(response.Unsupported);
    }

    [Fact]
    public async Task TextToSqlEnabled_EmptyOrderResult_ShouldReturnSupportedEmptyState()
    {
        var orchestrator = CreateOrchestrator(
            new RecordingSender(_ => throw new InvalidOperationException("Existing assistant path should not be called.")),
            textToSqlPlanner: new RecordingTextToSqlPlanner(OrdersPlan()),
            textToSqlExecutor: new RecordingTextToSqlExecutor(new AssistantSqlResult(
                true,
                ["OrderId", "Status", "TotalAmount", "CreatedAt", "LineCount"],
                Array.Empty<AssistantSqlRow>(),
                0,
                false)),
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("show my recent orders", Guid.NewGuid(), CancellationToken.None);

        Assert.False(response.Unsupported);
        Assert.Equal(AssistantResponseTypes.RecentOrders, response.ResponseType);
        Assert.Contains("do not have any recent orders", response.Answer, StringComparison.OrdinalIgnoreCase);

        var data = Assert.IsType<AssistantOrdersData>(response.Data);
        Assert.Empty(data.Orders);
    }

    [Fact]
    public async Task TextToSqlEnabled_GenericTable_ShouldFallBackToExistingAssistantPath()
    {
        var sender = new RecordingSender(request => request switch
        {
            ListOrdersForBuyerQuery query => new OrdersPagedResult(Array.Empty<OrderSummaryDto>(), query.PageNumber ?? 1, query.PageSize ?? 100, 0),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var planner = new RecordingTextToSqlPlanner(new AssistantTextToSqlPlan(
            true,
            AssistantSqlDataSource.Orders,
            "SELECT TOP (1) OrderId FROM assistant.v_MyOrders WHERE BuyerUserId = @CurrentUserId",
            AssistantTextToSqlResultShape.GenericTable,
            null));
        var orchestrator = CreateOrchestrator(
            sender,
            textToSqlPlanner: planner,
            textToSqlExecutor: new RecordingTextToSqlExecutor(new AssistantSqlResult(
                true,
                ["OrderId"],
                [new AssistantSqlRow(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["OrderId"] = Guid.NewGuid()
                })],
                1,
                false)),
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("show my recent orders", Guid.NewGuid(), CancellationToken.None);

        Assert.Single(sender.Requests);
        Assert.False(response.Unsupported);
        Assert.NotEqual("genericTable", response.ResponseType);
    }

    [Fact]
    public async Task TextToSqlEnabled_WriteRequest_ShouldRemainUnsupported()
    {
        var sender = new RecordingSender(_ => throw new InvalidOperationException("Sender should not be called."));
        var planner = new RecordingTextToSqlPlanner(AssistantTextToSqlPlan.Unsupported());
        var orchestrator = CreateOrchestrator(
            sender,
            textToSqlPlanner: planner,
            textToSqlExecutor: new RecordingTextToSqlExecutor(AssistantSqlResult.Failure()),
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("deactivate product", Guid.NewGuid(), CancellationToken.None);

        Assert.True(response.Unsupported);
        Assert.Empty(sender.Requests);
    }

    [Fact]
    public async Task TextToSqlEnabled_ShouldNotIncludeGeneratedSqlInResponse()
    {
        var sql = OrdersPlan().Sql!;
        var orchestrator = CreateOrchestrator(
            new RecordingSender(_ => throw new InvalidOperationException("Existing assistant path should not be called.")),
            textToSqlPlanner: new RecordingTextToSqlPlanner(OrdersPlan()),
            textToSqlExecutor: new RecordingTextToSqlExecutor(OrderListResult()),
            textToSqlEnabled: true);

        var response = await orchestrator.QueryAsync("what is my last order", Guid.NewGuid(), CancellationToken.None);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.DoesNotContain(sql, json, StringComparison.Ordinal);
        Assert.DoesNotContain("SELECT", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("assistant.v_MyOrders", json, StringComparison.OrdinalIgnoreCase);
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

    [Fact]
    public void OrdersAssistantSubAgent_ShouldStayInsideAllowedApiLayerBoundary()
    {
        var root = ProjectGraph.GetRootPath();
        var source = File.ReadAllText(Path.Combine(root, "src", "Api", "Ecommerce.Api", "Assistant", "OrdersAssistantSubAgent.cs"));

        Assert.Contains("ISender", source, StringComparison.Ordinal);
        Assert.Contains("ListOrdersForBuyerQuery", source, StringComparison.Ordinal);
        Assert.Contains("GetOrderByIdQuery", source, StringComparison.Ordinal);
        Assert.DoesNotContain("DbContext", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Repository", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Ecommerce.Orders.Domain", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Ecommerce.Orders.Infrastructure", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Ecommerce.Api.Mcp", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Gemini", source, StringComparison.Ordinal);
        Assert.DoesNotContain("HttpAssistantLlmClient", source, StringComparison.Ordinal);
        Assert.DoesNotContain("TextToSql", source, StringComparison.Ordinal);
        Assert.DoesNotContain("CreateOrderCommand", source, StringComparison.Ordinal);
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
        IAssistantIntentInterpreter? interpreter = null,
        IAssistantTextToSqlPlanner? textToSqlPlanner = null,
        IAssistantReadOnlySqlExecutor? textToSqlExecutor = null,
        bool textToSqlEnabled = false)
    {
        var safetyPolicy = new AssistantSafetyPolicy();
        var intentRouter = new AssistantIntentRouter(safetyPolicy);
        var deterministicInterpreter = new DeterministicAssistantIntentInterpreter(intentRouter);
        var toolRegistry = new AssistantToolRegistry();
        var ordersAssistantSubAgent = new OrdersAssistantSubAgent(sender, toolRegistry);

        return new AssistantOrchestrator(
            sender,
            ordersAssistantSubAgent,
            interpreter ?? deterministicInterpreter,
            deterministicInterpreter,
            new AssistantIntentPlanValidator(toolRegistry, safetyPolicy),
            textToSqlPlanner ?? new RecordingTextToSqlPlanner(AssistantTextToSqlPlan.Unsupported()),
            CreateSqlValidator(),
            textToSqlExecutor ?? new RecordingTextToSqlExecutor(AssistantSqlResult.Failure()),
            new AssistantTextToSqlResponseMapper(),
            NullLogger<AssistantOrchestrator>.Instance,
            Options.Create(new AssistantLlmOptions()),
            Options.Create(new AssistantTextToSqlOptions
            {
                Enabled = textToSqlEnabled,
                MaxRows = 50,
                CommandTimeoutSeconds = 5
            }));
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
            Provider = AssistantLlmOptions.OpenAiProvider,
            Endpoint = "https://example.test/v1/responses",
            Model = "test-model",
            ApiKey = "test-api-key",
            ApiKeyEnvironmentVariable = string.Empty,
            TimeoutSeconds = 5,
            MaxResponseCharacters = 4000
        };

    private static AssistantLlmOptions CreateEnabledGeminiOptions() =>
        new()
        {
            Enabled = true,
            Provider = AssistantLlmOptions.GeminiProvider,
            GeminiEndpoint = "https://example.test/v1beta",
            GeminiModel = "gemini-test",
            GeminiApiKey = "test-gemini-key",
            GeminiApiKeyEnvironmentVariable = string.Empty,
            TimeoutSeconds = 5,
            MaxResponseCharacters = 4000
        };

    private static AssistantSqlValidator CreateSqlValidator(int maxRows = 50) =>
        new(Options.Create(new AssistantTextToSqlOptions
        {
            Enabled = true,
            MaxRows = maxRows,
            CommandTimeoutSeconds = 5
        }));

    private static AssistantReadOnlySqlExecutor CreateSqlExecutor(
        DbConnection connection,
        int maxRows = 50,
        int timeoutSeconds = 5) =>
        new(
            CreateSqlValidator(maxRows),
            new StubSqlConnectionFactory(connection),
            Options.Create(new AssistantTextToSqlOptions
            {
                Enabled = true,
                MaxRows = maxRows,
                CommandTimeoutSeconds = timeoutSeconds
            }),
            NullLogger<AssistantReadOnlySqlExecutor>.Instance);

    private static LlmAssistantTextToSqlPlanner CreateTextToSqlPlanner(
        IAssistantLlmClient client,
        bool enabled = true) =>
        new(
            client,
            new AssistantTextToSqlPromptBuilder(),
            new AssistantTextToSqlPlanParser(),
            Options.Create(new AssistantLlmOptions
            {
                Enabled = enabled,
                Endpoint = "https://example.test/v1/responses",
                Model = "test-model",
                ApiKey = "test-api-key",
                ApiKeyEnvironmentVariable = string.Empty,
                TimeoutSeconds = 5,
                MaxResponseCharacters = 4000
            }),
            NullLogger<LlmAssistantTextToSqlPlanner>.Instance);

    private sealed class ScopedAssistantEnvironment : IDisposable
    {
        private static readonly string[] VariableNames =
        [
            "ECOMMERCE_ASSISTANT_LLM_PROVIDER",
            "ECOMMERCE_ASSISTANT_GEMINI_ENDPOINT",
            "ECOMMERCE_ASSISTANT_GEMINI_MODEL",
            "ECOMMERCE_ASSISTANT_GEMINI_API_KEY",
            "ECOMMERCE_ASSISTANT_TEXT_TO_SQL_ENABLED",
            "Assistant__TextToSql__Enabled",
            "ConnectionStrings__AssistantCatalogReadOnly",
            "ConnectionStrings__AssistantOrdersReadOnly"
        ];

        private readonly Dictionary<string, string?> previousValues;

        private ScopedAssistantEnvironment(Dictionary<string, string?> previousValues)
        {
            this.previousValues = previousValues;
        }

        public static ScopedAssistantEnvironment Clear()
        {
            var previousValues = new Dictionary<string, string?>(StringComparer.Ordinal);

            foreach (var variableName in VariableNames)
            {
                previousValues[variableName] = Environment.GetEnvironmentVariable(variableName);
                Environment.SetEnvironmentVariable(variableName, null);
            }

            return new ScopedAssistantEnvironment(previousValues);
        }

        public void Dispose()
        {
            foreach (var (variableName, value) in previousValues)
            {
                Environment.SetEnvironmentVariable(variableName, value);
            }
        }
    }

    private static AssistantTextToSqlPlan OrdersPlan(int top = 10) =>
        new(
            true,
            AssistantSqlDataSource.Orders,
            $"SELECT TOP ({top}) OrderId, Status, TotalAmount, CreatedAt, LineCount FROM assistant.v_MyOrders WHERE BuyerUserId = @CurrentUserId ORDER BY CreatedAt DESC",
            AssistantTextToSqlResultShape.OrderList,
            null);

    private static AssistantTextToSqlPlan OrderProductMatchPlan() =>
        new(
            true,
            AssistantSqlDataSource.Orders,
            "SELECT TOP (1) o.OrderId, o.Status, o.TotalAmount, o.CreatedAt, o.LineCount, l.ProductId, l.ProductName, l.ProductSku, l.Quantity, l.UnitPriceAmount, l.LineTotal FROM assistant.v_MyOrders AS o INNER JOIN assistant.v_MyOrderLines AS l ON l.OrderId = o.OrderId WHERE o.BuyerUserId = @CurrentUserId AND l.BuyerUserId = @CurrentUserId AND (l.ProductName LIKE '%Galaxy%' OR l.ProductSku LIKE '%Galaxy%') ORDER BY o.CreatedAt ASC",
            AssistantTextToSqlResultShape.OrderList,
            null);

    private static AssistantSqlResult OrderListResult()
    {
        var orderId = Guid.NewGuid();

        return new AssistantSqlResult(
            true,
            ["OrderId", "Status", "TotalAmount", "CreatedAt", "LineCount"],
            [new AssistantSqlRow(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["OrderId"] = orderId,
                ["Status"] = "Created",
                ["TotalAmount"] = 42.50m,
                ["CreatedAt"] = DateTimeOffset.UtcNow,
                ["LineCount"] = 2
            })],
            1,
            false);
    }

    private static AssistantSqlResult OrderProductMatchResult(int matchingLineRows = 1)
    {
        var orderId = Guid.NewGuid();
        var firstProductId = Guid.NewGuid();
        var secondProductId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddDays(-10);
        var rows = new List<AssistantSqlRow>
        {
            new(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["OrderId"] = orderId,
                ["Status"] = "Created",
                ["TotalAmount"] = 119.98m,
                ["CreatedAt"] = createdAt,
                ["LineCount"] = 2,
                ["ProductId"] = firstProductId,
                ["ProductName"] = "Galaxy Buds",
                ["ProductSku"] = "GALAXY-BUDS",
                ["Quantity"] = 1,
                ["UnitPriceAmount"] = 99.99m,
                ["LineTotal"] = 99.99m
            })
        };

        if (matchingLineRows > 1)
        {
            rows.Add(new AssistantSqlRow(new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["OrderId"] = orderId,
                ["Status"] = "Created",
                ["TotalAmount"] = 119.98m,
                ["CreatedAt"] = createdAt,
                ["LineCount"] = 2,
                ["ProductId"] = secondProductId,
                ["ProductName"] = "Galaxy Case",
                ["ProductSku"] = "GALAXY-CASE",
                ["Quantity"] = 1,
                ["UnitPriceAmount"] = 19.99m,
                ["LineTotal"] = 19.99m
            }));
        }

        return new AssistantSqlResult(
            true,
            [
                "OrderId",
                "Status",
                "TotalAmount",
                "CreatedAt",
                "LineCount",
                "ProductId",
                "ProductName",
                "ProductSku",
                "Quantity",
                "UnitPriceAmount",
                "LineTotal"
            ],
            rows,
            rows.Count,
            false);
    }

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

    private sealed class RecordingTextToSqlPlanner(AssistantTextToSqlPlan plan) : IAssistantTextToSqlPlanner
    {
        public List<string> Questions { get; } = [];

        public Task<AssistantTextToSqlPlan> PlanAsync(
            string question,
            CancellationToken cancellationToken)
        {
            Questions.Add(question);
            return Task.FromResult(plan);
        }
    }

    private sealed class RecordingTextToSqlExecutor(AssistantSqlResult result) : IAssistantReadOnlySqlExecutor
    {
        public List<AssistantSqlQuery> Queries { get; } = [];

        public Task<AssistantSqlResult> ExecuteAsync(
            AssistantSqlQuery query,
            CancellationToken cancellationToken)
        {
            Queries.Add(query);
            return Task.FromResult(result);
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

        public Uri? RequestUri { get; private set; }

        public HttpMethod? RequestMethod { get; private set; }

        public string? AuthorizationHeader { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            RequestMethod = request.Method;
            AuthorizationHeader = request.Headers.Authorization?.ToString();
            RequestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return handler(request);
        }
    }

    private sealed class StubSqlConnectionFactory(DbConnection connection) : IAssistantSqlConnectionFactory
    {
        public DbConnection CreateConnection(AssistantSqlDataSource dataSource) => connection;
    }

    private sealed class RecordingDbConnection(DbCommand command) : DbConnection
    {
        private ConnectionState _state = ConnectionState.Closed;

        [AllowNull]
        public override string ConnectionString { get; set; } = string.Empty;

        public override string Database => "Fake";

        public override string DataSource => "Fake";

        public override string ServerVersion => "1";

        public override ConnectionState State => _state;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
            _state = ConnectionState.Closed;
        }

        public override void Open()
        {
            _state = ConnectionState.Open;
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            _state = ConnectionState.Open;
            return Task.CompletedTask;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
            throw new NotSupportedException();

        protected override DbCommand CreateDbCommand() => command;
    }

    private sealed class RecordingDbCommand : DbCommand
    {
        private readonly DbDataReader? _reader;
        private readonly Exception? _exception;
        private readonly RecordingDbParameterCollection _parameters = new();

        public RecordingDbCommand(DbDataReader reader)
        {
            _reader = reader;
        }

        public RecordingDbCommand(Exception exception)
        {
            _exception = exception;
        }

        [AllowNull]
        public override string CommandText { get; set; } = string.Empty;

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection? DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection => _parameters;

        protected override DbTransaction? DbTransaction { get; set; }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery() => throw new NotSupportedException();

        public override object? ExecuteScalar() => throw new NotSupportedException();

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter() => new RecordingDbParameter();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (_exception is not null)
            {
                throw _exception;
            }

            return _reader ?? throw new InvalidOperationException("No reader configured.");
        }
    }

    private sealed class RecordingDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }

        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        public override bool IsNullable { get; set; }

        [AllowNull]
        public override string ParameterName { get; set; } = string.Empty;

        [AllowNull]
        public override string SourceColumn { get; set; } = string.Empty;

        public override object? Value { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override int Size { get; set; }

        public override void ResetDbType()
        {
        }
    }

    private sealed class RecordingDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _parameters = [];

        public override int Count => _parameters.Count;

        public override object SyncRoot => ((ICollection)_parameters).SyncRoot;

        public override int Add(object value)
        {
            _parameters.Add((DbParameter)value);
            return _parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }

        public override void Clear() => _parameters.Clear();

        public override bool Contains(object value) => _parameters.Contains((DbParameter)value);

        public override bool Contains(string value) =>
            _parameters.Any(parameter => string.Equals(parameter.ParameterName, value, StringComparison.Ordinal));

        public override void CopyTo(Array array, int index) =>
            ((ICollection)_parameters).CopyTo(array, index);

        public override IEnumerator GetEnumerator() => _parameters.GetEnumerator();

        public override int IndexOf(object value) => _parameters.IndexOf((DbParameter)value);

        public override int IndexOf(string parameterName) =>
            _parameters.FindIndex(parameter => string.Equals(parameter.ParameterName, parameterName, StringComparison.Ordinal));

        public override void Insert(int index, object value) => _parameters.Insert(index, (DbParameter)value);

        public override void Remove(object value) => _parameters.Remove((DbParameter)value);

        public override void RemoveAt(int index) => _parameters.RemoveAt(index);

        public override void RemoveAt(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        protected override DbParameter GetParameter(int index) => _parameters[index];

        protected override DbParameter GetParameter(string parameterName) =>
            _parameters[IndexOf(parameterName)];

        protected override void SetParameter(int index, DbParameter value) => _parameters[index] = value;

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index >= 0)
            {
                _parameters[index] = value;
                return;
            }

            _parameters.Add(value);
        }
    }

    private sealed class FakeDbDataReader(
        IReadOnlyList<string> columns,
        IReadOnlyList<object?[]> rows) : DbDataReader
    {
        private int _rowIndex = -1;

        public override int FieldCount => columns.Count;

        public override bool HasRows => rows.Count > 0;

        public override bool IsClosed => false;

        public override int RecordsAffected => 0;

        public override int Depth => 0;

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override bool Read()
        {
            if (_rowIndex + 1 >= rows.Count)
            {
                return false;
            }

            _rowIndex++;
            return true;
        }

        public override Task<bool> ReadAsync(CancellationToken cancellationToken) =>
            Task.FromResult(Read());

        public override bool NextResult() => false;

        public override string GetName(int ordinal) => columns[ordinal];

        public override int GetOrdinal(string name)
        {
            for (var index = 0; index < columns.Count; index++)
            {
                if (string.Equals(columns[index], name, StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }

            return -1;
        }

        public override object GetValue(int ordinal) => rows[_rowIndex][ordinal]!;

        public override bool IsDBNull(int ordinal) => GetValue(ordinal) is null or DBNull;

        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) =>
            Task.FromResult(IsDBNull(ordinal));

        public override string GetDataTypeName(int ordinal) => GetFieldType(ordinal).Name;

        public override Type GetFieldType(int ordinal) =>
            rows.Count == 0 || rows[0][ordinal] is null ? typeof(object) : rows[0][ordinal]!.GetType();

        public override int GetValues(object[] values)
        {
            var count = Math.Min(values.Length, FieldCount);
            for (var index = 0; index < count; index++)
            {
                values[index] = GetValue(index);
            }

            return count;
        }

        public override IEnumerator GetEnumerator() => rows.GetEnumerator();

        public override bool GetBoolean(int ordinal) => (bool)GetValue(ordinal);

        public override byte GetByte(int ordinal) => (byte)GetValue(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) =>
            throw new NotSupportedException();

        public override char GetChar(int ordinal) => (char)GetValue(ordinal);

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) =>
            throw new NotSupportedException();

        public override Guid GetGuid(int ordinal) => (Guid)GetValue(ordinal);

        public override short GetInt16(int ordinal) => (short)GetValue(ordinal);

        public override int GetInt32(int ordinal) => (int)GetValue(ordinal);

        public override long GetInt64(int ordinal) => (long)GetValue(ordinal);

        public override float GetFloat(int ordinal) => (float)GetValue(ordinal);

        public override double GetDouble(int ordinal) => (double)GetValue(ordinal);

        public override string GetString(int ordinal) => (string)GetValue(ordinal);

        public override decimal GetDecimal(int ordinal) => (decimal)GetValue(ordinal);

        public override DateTime GetDateTime(int ordinal) => (DateTime)GetValue(ordinal);
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
