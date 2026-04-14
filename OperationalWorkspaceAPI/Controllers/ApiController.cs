using FluentValidation;
using Microsoft.AspNetCore.Mvc;
// Namespaces updated to match your folder structure
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceShared.Validators;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Controllers;

#region Base Controller
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

    protected IActionResult Failure(string? message = null, int statusCode = 400)
        => StatusCode(statusCode, new
        {
            success = false,
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

    // Helper to format FluentValidation errors for the UI
    protected IActionResult ValidationFailure(FluentValidation.Results.ValidationResult result)
    {
        var errors = result.Errors.Select(e => new
        {
            property = e.PropertyName,
            message = e.ErrorMessage
        });

        return BadRequest(new
        {
            success = false,
            error = "Validation failed",
            details = errors,
            traceId = TraceId,
            timestamp = DateTime.UtcNow
        });
    }

    protected IActionResult HandleResult(dynamic result)
    {
        if (result == null) return Failure("Service returned no result.");

        return result.IsSuccess
            ? Success(result.Value)
            : Failure(result.Error);
    }
}
#endregion

