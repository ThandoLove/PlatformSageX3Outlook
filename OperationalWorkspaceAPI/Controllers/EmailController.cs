using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.DTOs;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Controllers;

public sealed class EmailController : ApiController
{
    private readonly IEmailService _service;

    public EmailController(IEmailService service) => _service = service;

    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Send([FromBody] EmailInsightDto dto)
    {
        // FIX: Call the actual method name from your interface
        var isSynced = await _service.SyncEmailAsync(dto);

        if (!isSynced)
        {
            return Failure("Failed to sync email.");
        }

        return Accepted(new
        {
            success = true,
            // Note: Since SyncEmailAsync returns bool, there is no taskId. 
            // Using TraceId for tracking instead.
            traceId = TraceId
        });
    }
}
