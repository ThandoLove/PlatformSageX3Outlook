using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/email")]
    public sealed class EmailController : ControllerBase
    {
        private readonly IEmailService _service;

        public EmailController(IEmailService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // ---------------------------------------------------------------------
        // 1. SYNC EMAIL FROM OUTLOOK
        // ---------------------------------------------------------------------
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

            // Return the canonical Outlook message id so the client can use it for subsequent context calls
            return Accepted(new
            {
                success = true,
                message = "Email synced successfully",
                messageId = dto.MessageId
            });
        }

        // ---------------------------------------------------------------------
        // 2. GET EMAIL (BASIC) - accepts Outlook message id string
        // ---------------------------------------------------------------------
        [HttpGet("{emailId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string emailId)
        {
            if (string.IsNullOrWhiteSpace(emailId))
                return BadRequest("Invalid message identifier.");

            var email = await _service.GetEmailByIdAsync(emailId);

            if (email == null)
                return NotFound();

            return Ok(email);
        }

        // ---------------------------------------------------------------------
        // 3. ⭐ MAIN ENDPOINT (INTELLIGENCE LAYER - FIXED CONFIGURATION)
        // ---------------------------------------------------------------------
        [HttpGet("{emailId}/context")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetContext(string emailId)
        {
            if (string.IsNullOrWhiteSpace(emailId))
                return BadRequest("Invalid message tracking identifier.");
            // Delegate to service using Outlook message id
            var context = await _service.GetEmailContextAsync(emailId);

            // 🔥 FIX: Check if context is completely empty or matched with an Un-registered/Unknown profile
            if (context == null || context.Email == null || string.IsNullOrWhiteSpace(context.Email.BusinessPartnerCode) || context.Email.BusinessPartnerCode == "Unknown")
            {
                // Returns an explicitly flag to cleanly trigger the "Add to Sage X3" modal on the client side
                return Ok(new
                {
                    isUnknownSender = true,
                    payload = context
                });
            }

            return Ok(new
            {
                isUnknownSender = false,
                payload = context
            });
        }
    }

    [ApiController]
    [Route("api/testemail")]
    public class TestEmailController : ControllerBase
    {
        [HttpGet("latest")]
        public IActionResult Latest()
        {
            return Ok(new
            {
                // Stable tracking hash ensures local browser mode runs safely without infinite timer loops
                messageId = "MOCK_SAGE_X3_TEST_CONVERSATION",
                senderName = "Unknown Entity Demo",
                senderEmail = "prospect-demo@testcompany.com",
                subject = "Requesting Urgent Product Catalog Pricing Info"
            });
        }
    }
}
