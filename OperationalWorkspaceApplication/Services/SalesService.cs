// ISalesService.cs
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using OperationalWorkspaceApplication.Interfaces.IServices;


// SalesService.cs
namespace OperationalWorkspaceApplication.Services;

public sealed class SalesService : ISalesService
{
    private readonly ISalesOrderRepository _salesRepo;

    public SalesService(ISalesOrderRepository salesRepo) => _salesRepo = salesRepo;

    public async Task<int> CountOpenOrdersAsync(string userId) =>
        await _salesRepo.CountByStatusAsync(userId, "Open", CancellationToken.None);

    public async Task<int> CountPendingDeliveriesAsync(string userId) =>
        await _salesRepo.CountByStatusAsync(userId, "Pending", CancellationToken.None);

    public async Task<int> CountTotalOrdersAsync() =>
        await _salesRepo.CountTotalAsync(CancellationToken.None);

    public async Task<SalesOrderDetailsResponse?> GetOrderAsync(GetSalesOrderRequest request, CancellationToken ct)
    {
        var order = await _salesRepo.GetByIdAsync(request.OrderId, ct);
        if (order is null) return null;

        var dto = new SalesOrderDto
        {
            Id = order.Id,
            BusinessPartnerCode = order.BpCode,
            TotalAmount = order.TotalAmount,
            OrderDate = DateTime.UtcNow,
            OrderStatus = "Open"
        };

        return new SalesOrderDetailsResponse(dto);
    }
}
