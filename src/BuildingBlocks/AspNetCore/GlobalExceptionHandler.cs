using FluentValidation;
using Marketplace.Application;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Marketplace.AspNetCore;

/// <summary>Maps application exceptions to RFC 7807 ProblemDetails responses (API-4, API-6).</summary>
public sealed partial class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "One or more validation errors occurred."),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found."),
            ConflictException => (StatusCodes.Status409Conflict, "The request could not be completed due to a conflict."),
            ForbiddenException => (StatusCodes.Status403Forbidden, "You do not have permission to perform this action."),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "The resource was modified by another request."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred."),
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            LogUnhandledException(logger, httpContext.Request.Path.ToString(), exception);
        }

        httpContext.Response.StatusCode = status;

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            // Never leak internal detail on a 500 (API-6, SEC).
            Detail = status == StatusCodes.Status500InternalServerError ? null : SafeDetail(exception),
        };
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (exception is ValidationException validation)
        {
            problemDetails.Extensions["errors"] = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception,
        });
    }

    private static string? SafeDetail(Exception exception) =>
        exception is ValidationException ? null : exception.Message;

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception processing {Path}")]
    private static partial void LogUnhandledException(ILogger logger, string path, Exception exception);
}
