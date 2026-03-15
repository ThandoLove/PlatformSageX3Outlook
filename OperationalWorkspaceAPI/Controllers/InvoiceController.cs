using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceAPI.Controllers

{
    public sealed class InvoiceController : ApiController
    {
        private readonly IInvoiceService _service;
        public InvoiceController(IInvoiceService service) => _service = service;
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var invoice = await _service.GetByIdAsync(id);
            return invoice == null ? NotFoundResponse("Invoice not found") : Success(invoice);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            if (pageSize > 100) pageSize = 100; // Shield: Prevent excessive data retrieval
            var invoices = await _service.GetAllAsync(page, pageSize);
            return Success(invoices);
        }

        //Controllers/InvoiceController.cs
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceDto dto)
        {
            var result = await _service.CreateInvoiceAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, Success(result));
        }
    }
}