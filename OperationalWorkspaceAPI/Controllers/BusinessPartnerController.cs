using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.DTOs; // Added to map ClientDto
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Controllers
{
    [ApiController]
    [Route("api/businesspartner")] // Explicit base route ensuring predictable client navigation paths
    public sealed class BusinessPartnerController : ApiController
    {
        private readonly IBusinessPartnerService _service;

        public BusinessPartnerController(IBusinessPartnerService service) => _service = service;

        [HttpGet("snapshot")]
        public async Task<IActionResult> GetSnapshot([FromQuery] string? bpCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        {
            if (pageSize > 200) pageSize = 200;

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
            var result = await _service.UpdateCreditLimitAsync(request, ct);
            return Success(result);
        }

        [HttpPost("from-email")]
        public async Task<IActionResult> CreateFromEmail([FromBody] CreateClientFromEmailRequest request)
        {
            var result = await _service.CreateFromEmailAsync(request);
            return Success(result);
        }

        // 🔥 NEW CRITICAL INGESTION ROUTE FOR YOUR CREATECLIENT.RAZOR WIZARD
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ClientDto clientDto)
        {
            if (clientDto == null) return BadRequest("Client properties cannot be empty.");

            // Dispatches execution downwards to run inside the thread-safe background sync workflow engine
            var success = await _service.CreateNewSageClientAsync(clientDto);

            if (!success) return BadRequest("Failed to delegate customer configuration down to background workers.");
            return Ok(new { success = true, message = "Customer added to Sage X3 tracking queues successfully." });
        }
    }
}
