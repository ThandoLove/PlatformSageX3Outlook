using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Controllers;

public sealed class InventoryController : ApiController
{
    private readonly IInventoryService _service;

    public InventoryController(IInventoryService service) => _service = service;

    [HttpGet("{id:guid}")] // Changed from string to Guid to match interface
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        // FIX: Pass both the Guid ID and the CancellationToken
        var item = await _service.GetItemAsync(id, ct);

        if (item == null)
            return NotFoundResponse("Item not found");

        return Success(item);
    }

    [HttpGet("availability")]
    public async Task<IActionResult> CheckAvailability([FromQuery] CheckStockRequest request, CancellationToken ct)
    {
        // Example of using the CheckAvailabilityAsync method from your interface
        var availability = await _service.CheckAvailabilityAsync(request, ct);
        return Success(availability);
    }
}
