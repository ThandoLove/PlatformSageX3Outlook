using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using OperationalWorkspace.Domain.Exceptions; // FIX: Import your specific logic

namespace OperationalWorkspaceAPI.Filters;

public sealed class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        var traceId = context.HttpContext.TraceIdentifier;

        // Categorize the failure for production response
        (int statusCode, string message) = context.Exception switch
        {
            // Shield: Specific Business Logic Failures (422 is better for logic errors)
            CreditLimitExceededException ex => (StatusCodes.Status422UnprocessableEntity, ex.Message),
            InsufficientStockException ex => (StatusCodes.Status422UnprocessableEntity, ex.Message),
            BusinessRuleException ex => (StatusCodes.Status400BadRequest, ex.Message),

            // Standard Infrastructure Failures
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorised access to this resource"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "The requested resource was not found"),

            // Global Catch-all
            _ => (StatusCodes.Status500InternalServerError, "An internal error occurred. Please provide Trace ID to support.")
        };

        // Production Shield: Log full stack trace for internal troubleshooting
        _logger.LogError(context.Exception, "API Error {TraceId} | Status: {Status} | Message: {Msg}",
            traceId, statusCode, context.Exception.Message);

        context.Result = new ObjectResult(new
        {
            success = false,
            error = message,
            traceId = traceId,
            timestamp = DateTime.UtcNow
        })
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
