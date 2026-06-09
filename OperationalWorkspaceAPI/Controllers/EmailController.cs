using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Controllers
{
    [ApiController]
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

            return Accepted(new
            {
                success = true,
                message = "Email synced successfully"
            });
        }

        // ---------------------------------------------------------------------
        // 2. GET EMAIL (BASIC)
        // ---------------------------------------------------------------------
        [HttpGet("{emailId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid emailId)
        {
            if (emailId == Guid.Empty)
                return BadRequest("Invalid cryptographic transaction identifier.");

            // Standard string-mapping compatibility layer conversion
            var email = await _service.GetEmailByIdAsync(emailId);

            if (email == null)
                return NotFound();

            return Ok(email);
        }

        // ---------------------------------------------------------------------
        // 3. ⭐ MAIN ENDPOINT (INTELLIGENCE LAYER - FIXED MAPPING)
        // ---------------------------------------------------------------------
        [HttpGet("{emailId}/context")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContext(Guid emailId)
        {
            if (emailId == Guid.Empty)
                return BadRequest("Invalid cryptographic transaction identifier.");

            // FIX: Corrected target method link to tap straight into the context validation pipeline
            var context = await _service.GetEmailContextAsync(emailId);

            if (context == null)
            {
                // If not found, it passes an identifier down to naturally trigger 
                // the "Add to Sage X3" client creation modal flow on the UI side
                return Ok(new { isUnknownSender = true });
            }

            return Ok(context);
        }
    }

    //[ApiController]
    //[Route("api/testemail")]
    //public class TestEmailController : ControllerBase
    //{
      //  [HttpGet("latest")]
        //public IActionResult Latest()
        //{
          //  return Ok(new
            //{
              //  messageId = Guid.NewGuid().ToString(),
                //senderName = "Demo User",
                //senderEmail = "demo@test.com",
                //subject = "Demo Email"
            //});
        //}
    //}


}
