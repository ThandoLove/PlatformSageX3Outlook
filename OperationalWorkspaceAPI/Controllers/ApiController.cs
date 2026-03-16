using Microsoft.AspNetCore.Mvc;
using System;

namespace OperationalWorkspaceAPI.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/v1/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected string TraceId => HttpContext.TraceIdentifier;

    protected IActionResult Success<T>(T data)
        => Ok(new
        {
            success = true,
            data,
            traceId = TraceId,
            timestamp = DateTime.UtcNow
        });

    // FIX: Added '?' to string message to allow nulls, then handle them internally
    protected IActionResult Failure(string? message = null, int statusCode = 400)
        => StatusCode(statusCode, new
        {
            success = false,
            // Fallback to a generic message if the service returns a null error
            error = message ?? "An unexpected error occurred.",
            traceId = TraceId,
            timestamp = DateTime.UtcNow
        });

    protected IActionResult NotFoundResponse(string? message = null)
        => NotFound(new
        {
            success = false,
            error = message ?? "The requested resource was not found.",
            traceId = TraceId,
            timestamp = DateTime.UtcNow
        });

    // NEW: Helper to clean up your Controller code
    // This allows you to just write: return HandleResult(result);
    protected IActionResult HandleResult<T>(dynamic result)
    {
        if (result == null) return Failure("Service returned no result.");

        return result.IsSuccess
            ? Success(result.Value)
            : Failure(result.Error);
    }
}
