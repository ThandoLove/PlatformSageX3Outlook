using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;

namespace OperationalWorkspaceAPI.Controllers;

public sealed class SalesOrderController : ApiController
{
    private readonly ISalesService _service;

    public SalesOrderController(ISalesService service) => _service = service;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        // FIX: Use GetOrderAsync and the required Request record
        var request = new GetSalesOrderRequest(id);
        var order = await _service.GetOrderAsync(request, ct);

        return order == null ? NotFoundResponse("Sales order not found") : Success(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSalesOrderRequest request, CancellationToken ct)
    {
        // FIX: Use CreateOrderAsync as defined in the interface
        var result = await _service.CreateOrderAsync(request, ct);

        // Accessing the Id from the CreateSalesOrderResponse record
        return CreatedAtAction(
            nameof(Get),
            new { id = result.OrderId }, // Ensure property name matches your response record
            Success(result));
    }
}
