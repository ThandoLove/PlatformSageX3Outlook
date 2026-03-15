using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace OperationalWorkspaceAPI.Controllers;


[ApiController]
[Produces("application/json")]
[Route("api/v1/[controller]")] // Added Versioning: Critical for production breaking changes
public abstract class ApiController : ControllerBase
{
    // Accessing the TraceIdentifier assigned by the Middleware
    protected string TraceId => HttpContext.TraceIdentifier;

    protected IActionResult Success<T>(T data)
        => Ok(new
        {
            success = true,
            data,
            traceId = TraceId, // Link response to server logs
            timestamp = DateTime.UtcNow
        });

    protected IActionResult Failure(string message, int statusCode = 400)
        => StatusCode(statusCode, new
        {
            success = false,
            error = message,
            traceId = TraceId,
            timestamp = DateTime.UtcNow
        });

    protected IActionResult NotFoundResponse(string message)
        => NotFound(new
        {
            success = false,
            error = message,
            traceId = TraceId,
            timestamp = DateTime.UtcNow
        });
}
