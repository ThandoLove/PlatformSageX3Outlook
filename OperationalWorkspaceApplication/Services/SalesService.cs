using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.ValueObjects;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Abstractions;

namespace OperationalWorkspace.Application.Services;

public sealed class SalesService : ISalesService
{
    private readonly ISalesOrderRepository _salesRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IUnitOfWork _uow;

    public SalesService(
        ISalesOrderRepository salesRepo,
        IInventoryRepository inventoryRepo,
        IUnitOfWork uow)
    {
        _salesRepo = salesRepo;
        _inventoryRepo = inventoryRepo;
        _uow = uow;
    }

    public async Task<int> CountOpenOrdersAsync(string userId) =>
        await _salesRepo.CountByStatusAsync(userId, "Open", CancellationToken.None);

    public async Task<int> CountPendingDeliveriesAsync(string userId) =>
        await _salesRepo.CountByStatusAsync(userId, "Pending", CancellationToken.None);

    public async Task<int> CountTotalOrdersAsync() =>
        await _salesRepo.CountTotalAsync(CancellationToken.None);

    public async Task<CreateSalesOrderResponse> CreateOrderAsync(CreateSalesOrderRequest request, CancellationToken ct)
    {
        var lines = new List<SalesOrderLine>();

        foreach (var l in request.Lines)
        {
            var item = await _inventoryRepo.GetBySkuAsync(l.Sku, ct)
                       ?? throw new InvalidOperationException($"SKU {l.Sku} not found");

            if (item.AvailableQuantity < l.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            item.Reserve(l.Quantity);
            lines.Add(new SalesOrderLine(l.Sku, l.Quantity, new Money(l.UnitPrice)));
        }

        var order = new SalesOrder(request.BpCode, lines);

        await _salesRepo.AddAsync(order, ct);
        await _uow.SaveChangesAsync(ct);

        return new CreateSalesOrderResponse(order.Id);
    }

    public async Task<SalesOrderDetailsResponse?> GetOrderAsync(GetSalesOrderRequest request, CancellationToken ct)
    {
        var order = await _salesRepo.GetByIdAsync(request.OrderId, ct);
        if (order is null) return null;

        // FIXED: Using object initializer {} because SalesOrderDto has 'init' properties
        var dto = new SalesOrderDto
        {
            Id = order.Id,
            BusinessPartnerCode = order.BpCode,
            TotalAmount = order.TotalAmount,
            OrderDate = DateTime.UtcNow, // Map to your entity's date if it exists
            OrderStatus = "Open"
        };

        return new SalesOrderDetailsResponse(dto);
    }
}
