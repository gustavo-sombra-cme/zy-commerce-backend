using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using Ecommerce.Api.Mcp;
using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using Ecommerce.Orders.Application.Orders.CreateOrder;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using NetArchTest.Rules;

namespace Ecommerce.ArchitectureTests;

public sealed class McpIntegrationTests
{
    [Fact]
    public void McpTools_ShouldRequireAuthorization()
    {
        Assert.Contains(
            typeof(EcommerceMcpTools).GetCustomAttributes<AuthorizeAttribute>(),
            attribute => attribute.GetType() == typeof(AuthorizeAttribute));
    }

    [Fact]
    public void McpTools_ShouldExposeOnly_ApprovedAllowlist()
    {
        var toolNames = typeof(EcommerceMcpTools)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Select(method => method.GetCustomAttribute<McpServerToolAttribute>())
            .Where(attribute => attribute is not null)
            .Select(attribute => attribute!.Name)
            .OrderBy(name => name)
            .ToArray();

        Assert.Equal(
            new[]
            {
                "catalog_get_product_by_id",
                "catalog_search_products",
                "orders_create_order",
                "orders_get_order_by_id"
            },
            toolNames);
    }

    [Fact]
    public void McpEndpoint_ShouldBeMappedAsProtectedEndpoint()
    {
        var programPath = Path.Combine(ProjectGraph.GetRootPath(), "src", "Api", "Ecommerce.Api", "Program.cs");
        var programText = File.ReadAllText(programPath);

        Assert.Contains("app.MapMcp(\"/mcp\").RequireAuthorization();", programText);
    }

    [Fact]
    public void McpTools_ShouldDependOn_ISenderOnlyForApplicationDispatch()
    {
        var constructor = Assert.Single(typeof(EcommerceMcpTools).GetConstructors());
        var parameter = Assert.Single(constructor.GetParameters());

        Assert.Equal(typeof(ISender), parameter.ParameterType);
    }

    [Theory]
    [InlineData(nameof(EcommerceMcpTools.GetOrderById), "orderId")]
    [InlineData(nameof(EcommerceMcpTools.CreateOrder), "confirmedByUser")]
    public void McpOrderToolSchemas_ShouldNotExposeAuthenticatedUserContext(string methodName, string expectedArgumentName)
    {
        var method = typeof(EcommerceMcpTools).GetMethod(methodName)
            ?? throw new InvalidOperationException($"Could not find MCP tool method {methodName}.");
        var tool = McpServerTool.Create(
            method,
            new EcommerceMcpTools(new RecordingSender(_ => null)),
            new McpServerToolCreateOptions());
        var inputSchema = tool.ProtocolTool.InputSchema.GetRawText();
        var propertyNames = JsonDocument.Parse(inputSchema)
            .RootElement
            .GetProperty("properties")
            .EnumerateObject()
            .Select(property => property.Name)
            .ToArray();

        Assert.Contains(expectedArgumentName, propertyNames);
        Assert.DoesNotContain("user", propertyNames, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("context", propertyNames, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void McpTypes_ShouldNotDependOn_EfRepositoriesDomainOrModuleInfrastructure()
    {
        var result = Types.InAssembly(typeof(EcommerceMcpTools).Assembly)
            .That()
            .ResideInNamespace("Ecommerce.Api.Mcp")
            .Should()
            .NotHaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Ecommerce.Catalog.Domain",
                "Ecommerce.Auth.Domain",
                "Ecommerce.Orders.Domain",
                "Ecommerce.Catalog.Infrastructure.Persistence",
                "Ecommerce.Auth.Infrastructure.Persistence",
                "Ecommerce.Orders.Infrastructure.Persistence",
                "Ecommerce.Catalog.Application.Products.IProductRepository",
                "Ecommerce.Orders.Application.Orders.IOrderRepository")
            .GetResult();

        Assert.True(result.IsSuccessful, "MCP adapter types must not call persistence, repositories, Domain, or module internals directly.");
    }

    [Fact]
    public async Task CatalogSearchProducts_ShouldDispatch_SearchProductsQuery()
    {
        var sender = new RecordingSender(request => request switch
        {
            SearchProductsQuery => new PagedResult<ProductListItemDto>(
                new[]
                {
                    new ProductListItemDto(
                        Guid.NewGuid(),
                        "SKU-1",
                        "Test Product",
                        "Description",
                        true,
                        DateTimeOffset.UtcNow)
                },
                2,
                5,
                11),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var tools = new EcommerceMcpTools(sender);

        var result = await tools.SearchCatalogProducts("test", true, 2, 5);

        var query = Assert.IsType<SearchProductsQuery>(Assert.Single(sender.Requests));
        Assert.Equal("test", query.SearchTerm);
        Assert.True(query.IsActive);
        Assert.Equal(2, query.PageNumber);
        Assert.Equal(5, query.PageSize);
        Assert.Single(result.Items);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task CatalogGetProductById_ShouldDispatch_GetProductByIdQuery()
    {
        var productId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            GetProductByIdQuery => new ProductDetailsDto(
                productId,
                "SKU-1",
                "Test Product",
                null,
                true,
                DateTimeOffset.UtcNow,
                null),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var tools = new EcommerceMcpTools(sender);

        var result = await tools.GetCatalogProductById(productId);

        var query = Assert.IsType<GetProductByIdQuery>(Assert.Single(sender.Requests));
        Assert.Equal(productId, query.ProductId);
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
    }

    [Fact]
    public async Task OrdersGetOrderById_ShouldDispatch_OwnerScopedQuery()
    {
        var buyerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            GetOrderByIdQuery => new OrderDetailsDto(
                orderId,
                buyerId,
                "Created",
                42.50m,
                DateTimeOffset.UtcNow,
                Array.Empty<OrderLineDetailsDto>()),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var tools = new EcommerceMcpTools(sender);

        var result = await tools.GetOrderById(CreateContext(buyerId), orderId);

        var query = Assert.IsType<GetOrderByIdQuery>(Assert.Single(sender.Requests));
        Assert.Equal(orderId, query.OrderId);
        Assert.Equal(buyerId, query.BuyerId);
        Assert.NotNull(result);
        Assert.Equal(buyerId, result.BuyerId);
    }

    [Fact]
    public async Task OrdersCreateOrder_ShouldRequire_ConfirmedByUser()
    {
        var sender = new RecordingSender(_ => throw new InvalidOperationException("Sender should not be called."));
        var tools = new EcommerceMcpTools(sender);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            tools.CreateOrder(
                CreateContext(Guid.NewGuid()),
                confirmedByUser: false,
                new[]
                {
                    new McpCreateOrderLineInput(Guid.NewGuid(), "SKU-1", "Test Product", 9.99m, 1)
                }));

        Assert.Empty(sender.Requests);
    }

    [Fact]
    public async Task OrdersCreateOrder_ShouldDispatch_CreateOrderCommand_ForAuthenticatedUser()
    {
        var buyerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var sender = new RecordingSender(request => request switch
        {
            CreateOrderCommand => new CreateOrderResult(orderId, 19.98m, DateTimeOffset.UtcNow),
            _ => throw new InvalidOperationException($"Unexpected request {request.GetType().Name}.")
        });
        var tools = new EcommerceMcpTools(sender);

        var result = await tools.CreateOrder(
            CreateContext(buyerId),
            confirmedByUser: true,
            new[]
            {
                new McpCreateOrderLineInput(productId, "SKU-1", "Test Product", 9.99m, 2)
            });

        var command = Assert.IsType<CreateOrderCommand>(Assert.Single(sender.Requests));
        var line = Assert.Single(command.Lines);
        Assert.Equal(buyerId, command.BuyerId);
        Assert.Equal(productId, line.ProductId);
        Assert.Equal("SKU-1", line.ProductSku);
        Assert.Equal("Test Product", line.ProductName);
        Assert.Equal(9.99m, line.UnitPrice);
        Assert.Equal(2, line.Quantity);
        Assert.Equal(orderId, result.OrderId);
    }

    private static ClaimsPrincipal CreateUser(Guid userId)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim("sub", userId.ToString()) },
            authenticationType: "Test"));
    }

    private static RequestContext<CallToolRequestParams> CreateContext(Guid userId)
    {
        return new RequestContext<CallToolRequestParams>(
            new TestMcpServer(),
            new JsonRpcRequest
            {
                Method = RequestMethods.ToolsCall,
                Context = new JsonRpcMessageContext
                {
                    User = CreateUser(userId)
                }
            },
            new CallToolRequestParams
            {
                Name = "test"
            });
    }

#pragma warning disable MCPEXP002
    private sealed class TestMcpServer : McpServer
    {
        public override ClientCapabilities ClientCapabilities { get; } = new();

        public override Implementation ClientInfo { get; } = new()
        {
            Name = "test-client",
            Version = "1.0.0"
        };

        public override McpServerOptions ServerOptions { get; } = new();

        public override IServiceProvider Services { get; } = new EmptyServiceProvider();

        public override LoggingLevel? LoggingLevel => null;

        public override string SessionId => "test-session";

        public override string NegotiatedProtocolVersion => "2025-06-18";

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override Task<JsonRpcResponse> SendRequestAsync(
            JsonRpcRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public override Task SendMessageAsync(JsonRpcMessage message, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public override IAsyncDisposable RegisterNotificationHandler(
            string method,
            Func<JsonRpcNotification, CancellationToken, ValueTask> handler)
        {
            return EmptyAsyncDisposable.Instance;
        }

        public override ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
#pragma warning restore MCPEXP002

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return null;
        }
    }

    private sealed class EmptyAsyncDisposable : IAsyncDisposable
    {
        public static EmptyAsyncDisposable Instance { get; } = new();

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
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
}
