using System.Diagnostics;

namespace Ecommerce.Api.Middleware;

public sealed class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            logger.Log(
                GetLogLevel(statusCode),
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMilliseconds} ms.",
                context.Request.Method,
                context.Request.Path.Value,
                statusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }

    private static LogLevel GetLogLevel(int statusCode)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            return LogLevel.Error;
        }

        if (statusCode >= StatusCodes.Status400BadRequest)
        {
            return LogLevel.Warning;
        }

        return LogLevel.Information;
    }
}
