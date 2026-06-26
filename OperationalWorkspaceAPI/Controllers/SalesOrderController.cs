using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;

namespace OperationalWorkspaceAPI.Controllers;

public sealed class SalesOrderController : ApiController
{
    private readonly ISalesService _service;

    public SalesOrderController(ISalesService service) => _service = service;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var request = new GetSalesOrderRequest(id);
        var order = await _service.GetOrderAsync(request, ct);

        return order == null ? NotFoundResponse("Sales order not found") : Success(order);
    }
}
