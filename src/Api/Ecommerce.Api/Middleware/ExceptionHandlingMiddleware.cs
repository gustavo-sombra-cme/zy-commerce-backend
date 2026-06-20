using Ecommerce.Auth.Application.Users.LoginUser;
using Ecommerce.Auth.Application.Users.RegisterUser;
using Ecommerce.Catalog.Application.Products.CreateProduct;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            logger.LogInformation(
                "Validation failed for {RequestMethod} {RequestPath} with {ValidationErrorCount} validation errors.",
                context.Request.Method,
                context.Request.Path.Value,
                exception.Errors.Count());

            await WriteValidationProblemAsync(context, exception);
        }
        catch (DuplicateSkuException exception)
        {
            LogHandledException(context, exception, StatusCodes.Status409Conflict, LogLevel.Warning);

            await WriteProblemAsync(
                context,
                StatusCodes.Status409Conflict,
                "Conflict.",
                exception.Message);
        }
        catch (DuplicateEmailException exception)
        {
            LogHandledException(context, exception, StatusCodes.Status409Conflict, LogLevel.Warning);

            await WriteProblemAsync(
                context,
                StatusCodes.Status409Conflict,
                "Conflict.",
                exception.Message);
        }
        catch (InvalidCredentialsException exception)
        {
            LogHandledException(context, exception, StatusCodes.Status401Unauthorized, LogLevel.Warning);

            await WriteProblemAsync(
                context,
                StatusCodes.Status401Unauthorized,
                "Unauthorized.",
                exception.Message);
        }
        catch (InactiveUserException exception)
        {
            LogHandledException(context, exception, StatusCodes.Status403Forbidden, LogLevel.Warning);

            await WriteProblemAsync(
                context,
                StatusCodes.Status403Forbidden,
                "Forbidden.",
                exception.Message);
        }
        catch (KeyNotFoundException exception)
        {
            LogHandledException(context, exception, StatusCodes.Status404NotFound, LogLevel.Information);

            await WriteProblemAsync(
                context,
                StatusCodes.Status404NotFound,
                "Resource not found.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled API exception {ExceptionType} for {RequestMethod} {RequestPath}.",
                exception.GetType().Name,
                context.Request.Method,
                context.Request.Path.Value);

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteValidationProblemAsync(HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Type = "https://httpstatuses.com/400",
            Title = "Validation failed.",
            Status = StatusCodes.Status400BadRequest
        };

        await WriteValidationProblemDetailsAsync(context, problemDetails);
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string? detail = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Detail = detail,
            Status = statusCode
        };

        await WriteProblemDetailsAsync(context, problemDetails);
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, ProblemDetails problemDetails)
    {
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static async Task WriteValidationProblemDetailsAsync(
        HttpContext context,
        ValidationProblemDetails problemDetails)
    {
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private void LogHandledException(
        HttpContext context,
        Exception exception,
        int statusCode,
        LogLevel logLevel)
    {
        logger.Log(
            logLevel,
            "Handled API exception {ExceptionType} for {RequestMethod} {RequestPath} returned {StatusCode}.",
            exception.GetType().Name,
            context.Request.Method,
            context.Request.Path.Value,
            statusCode);
    }
}
