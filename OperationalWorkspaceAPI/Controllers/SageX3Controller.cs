
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceAPI.Controllers;

/// <summary>
/// Secure controller for Sage X3 operations.
/// Inherits from ApiController to use standardized Success/Failure responses.
/// </summary>
[Authorize] // Requires valid JWT Token
public class SageX3Controller : ApiController
{
    private readonly ISageRestService _sageService;
    private readonly ILogger<SageX3Controller> _logger;

    public SageX3Controller(ISageRestService sageService, ILogger<SageX3Controller> logger)
    {
        _sageService = sageService;
        _logger = logger;
    }

    /// <summary>
    /// Fetches customer list from Sage X3.
    /// URL: GET api/v1/SageX3/customers
    /// </summary>
    [HttpGet("customers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomers()
    {
        _logger.LogInformation("Attempting to fetch Sage X3 customers. TraceId: {TraceId}", TraceId);

        try
        {
            var result = await _sageService.GetCustomersAsync();

            // HandleResult is the helper in your ApiController base class
            // It automatically formats the Success(data) or Failure(message)
            return HandleResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical failure fetching Sage X3 customers. TraceId: {TraceId}", TraceId);
            return Failure("A critical error occurred while communicating with Sage X3.", 500);
        }
    }

    /// <summary>
    /// Fetches a specific business partner.
    /// URL: GET api/v1/SageX3/partner/{id}
    /// </summary>
    [HttpGet("partner/{id}")]
    public async Task<IActionResult> GetPartner(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Failure("Partner ID is required.");

        var result = await _sageService.GetPartnerByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Example of a protected POST action.
    /// URL: POST api/v1/SageX3/sync
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncData([FromBody] object syncPayload)
    {
        _logger.LogInformation("Data sync initiated by user {User}", User.Identity?.Name);

        // This is where your logic to push data TO Sage X3 would go.
        // Replace with actual service call
        return Success(new { message = "Sync initiated successfully" });
    }
}
