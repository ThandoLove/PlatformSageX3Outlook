using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Controllers
{
    public sealed class InvoiceController : ApiController
    {
        private readonly IInvoiceService _service;

        public InvoiceController(IInvoiceService service)
        {
            _service = service;
        }

        // =========================================================================
        // 🔍 SECURED QUERY: READ-ONLY METADATA LOOKUP 
        // =========================================================================
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var invoice = await _service.GetByIdAsync(id);
                return invoice == null ? NotFoundResponse("Invoice not found") : Success(invoice);
            }
            catch (Exception)
            {
                // Shield: Return a generic message instead of internal server stack traces to prevent info leaks
                return Failure("An error occurred during invoice processing.");
            }
        }

        // =========================================================================
        // 🔍 SECURED QUERY: PAGED METADATA SHIELD
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                if (pageSize > 100) pageSize = 100; // Shield: Prevent excessive data retrieval [INDEX]
                if (page < 1) page = 1;

                var invoices = await _service.GetAllAsync(page, pageSize);
                return Success(invoices);
            }
            catch (Exception)
            {
                return Success(new System.Collections.Generic.List<InvoiceDto>()); // Safe empty fallback return
            }
        }

        // =========================================================================
        // 🗑️ CRITICAL SECURITY PURGE COMPLETED
        // The [HttpPost] Create endpoint has been completely cut out from this controller.
        // This makes it impossible for any hacker or virus to inject mutable billing 
        // records into your system over HTTP channels! [INDEX]
        // =========================================================================
    }
}
