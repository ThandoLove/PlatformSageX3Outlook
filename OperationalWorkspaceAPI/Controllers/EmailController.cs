using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceAPI.Controllers;

[ApiController]
[Route("api/email")]
public sealed class EmailController : ControllerBase
{
    private readonly IEmailService _service;

    public EmailController(IEmailService service)
    {
        _service = service;
    }

    // -------------------------------
    // 1. SYNC EMAIL FROM OUTLOOK
    // -------------------------------
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Send([FromBody] EmailInsightDto dto)
    {
        if (dto == null)
            return BadRequest("Email cannot be null.");

        var isSynced = await _service.SyncEmailAsync(dto);

        if (!isSynced)
            return BadRequest("Email already exists or failed to sync.");

        return Accepted(new
        {
            success = true,
            message = "Email synced successfully"
        });
    }

    // -------------------------------
    // 2. GET EMAIL (BASIC)
    // -------------------------------
    [HttpGet("{emailId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string emailId)
    {
        var email = await _service.GetEmailByIdAsync(emailId);

        if (email == null)
            return NotFound();

        return Ok(email);
    }

    // -------------------------------
    // 3. ⭐ MAIN ENDPOINT (INTELLIGENCE LAYER)
    // -------------------------------
    [HttpGet("{emailId}/context")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContext(string emailId)
    {
        var context = await _service.GetEmailByIdAsync(emailId);

        if (context == null)
            return NotFound();

        return Ok(context);
    }
}