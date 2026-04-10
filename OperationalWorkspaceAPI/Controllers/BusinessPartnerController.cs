using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;

namespace OperationalWorkspaceAPI.Controllers;

public sealed class BusinessPartnerController : ApiController
{
    private readonly IBusinessPartnerService _service;

    public BusinessPartnerController(IBusinessPartnerService service) => _service = service;

    [HttpGet("snapshot")]
    public async Task<IActionResult> GetSnapshot(
    [FromQuery] string? bpCode, // Add BpCode from query string
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 50,
    CancellationToken ct = default)
    {
        if (pageSize > 200) pageSize = 200;

        // FIX: Match the 3-parameter constructor: (string BpCode, string Page, string PageSize)
        // Note: Use "" or a specific value if bpCode is null
        var request = new GetBusinessPartnerSnapshotRequest(
            bpCode ?? string.Empty,
            page.ToString(),
            pageSize.ToString()
        );

        var result = await _service.GetSnapshotAsync(request, ct);

        return result == null ? Failure("No snapshot found") : Success(result);
    }



    [HttpPost("update-credit-limit")]
    public async Task<IActionResult> UpdateCreditLimit([FromBody] UpdateCreditLimitRequest request, CancellationToken ct)
    {
        // FIX: Call UpdateCreditLimitAsync
        var result = await _service.UpdateCreditLimitAsync(request, ct);

        return Success(result);
    }

    [HttpPost("from-email")]
    public async Task<IActionResult> CreateFromEmail([FromBody] CreateClientFromEmailRequest request)
    {
        var result = await _service.CreateFromEmailAsync(request);
        return Success(result);
    }
}
