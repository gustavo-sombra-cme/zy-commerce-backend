using Ecommerce.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ecommerce.ArchitectureTests;

public sealed class StructuredLoggingMiddlewareTests
{
    [Fact]
    public async Task CorrelationIdMiddleware_PreservesIncomingCorrelationId()
    {
        const string correlationId = "existing-correlation-id";
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;

        var middleware = new CorrelationIdMiddleware(
            _ => Task.CompletedTask,
            NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);
        await context.Response.StartAsync();

        Assert.Equal(correlationId, context.TraceIdentifier);
        Assert.Equal(correlationId, context.Items[CorrelationIdMiddleware.HeaderName]);
        Assert.Equal(correlationId, context.Response.Headers[CorrelationIdMiddleware.HeaderName]);
    }

    [Fact]
    public async Task CorrelationIdMiddleware_GeneratesCorrelationId_WhenHeaderIsMissing()
    {
        var context = new DefaultHttpContext();

        var middleware = new CorrelationIdMiddleware(
            _ => Task.CompletedTask,
            NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);
        await context.Response.StartAsync();

        var correlationId = context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();

        Assert.False(string.IsNullOrWhiteSpace(correlationId));
        Assert.Equal(correlationId, context.TraceIdentifier);
        Assert.Equal(correlationId, context.Items[CorrelationIdMiddleware.HeaderName]);
    }

    [Fact]
    public async Task CorrelationIdMiddleware_ReturnsCorrelationId_ForErrorResponses()
    {
        const string correlationId = "error-correlation-id";
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;

        var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("Test exception."),
            NullLogger<ExceptionHandlingMiddleware>.Instance);
        var correlationIdMiddleware = new CorrelationIdMiddleware(
            exceptionHandlingMiddleware.InvokeAsync,
            NullLogger<CorrelationIdMiddleware>.Instance);

        await correlationIdMiddleware.InvokeAsync(context);
        await context.Response.StartAsync();

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal(correlationId, context.Response.Headers[CorrelationIdMiddleware.HeaderName]);
    }

    [Fact]
    public async Task RequestLoggingMiddleware_DoesNotLogAuthorizationHeaderValue()
    {
        const string authorizationHeader = "Bearer secret-token";
        var logger = new ListLogger<RequestLoggingMiddleware>();
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/api/auth/users/me";
        context.Request.Headers.Authorization = authorizationHeader;

        var middleware = new RequestLoggingMiddleware(
            requestContext =>
            {
                requestContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            logger);

        await middleware.InvokeAsync(context);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.LogLevel);
        Assert.Contains("HTTP GET /api/auth/users/me responded 401", entry.Message);
        Assert.DoesNotContain("Authorization", entry.Message);
        Assert.DoesNotContain(authorizationHeader, entry.Message);
        Assert.DoesNotContain("secret-token", entry.Message);
    }

    private sealed class ListLogger<T> : ILogger<T>
    {
        public List<LogEntry> Entries { get; } = new();

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull =>
            NoopDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add(new LogEntry(logLevel, formatter(state, exception)));
        }
    }

    private sealed record LogEntry(LogLevel LogLevel, string Message);

    private sealed class NoopDisposable : IDisposable
    {
        public static readonly NoopDisposable Instance = new();

        public void Dispose()
        {
        }
    }
}
