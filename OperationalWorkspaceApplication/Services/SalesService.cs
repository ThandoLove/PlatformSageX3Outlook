using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.ValueObjects;
using OperationalWorkspaceApplication.Abstractions;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;

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

    public async Task<CreateSalesOrderResponse> CreateOrderAsync(
        CreateSalesOrderRequest request,
        CancellationToken ct)
    {
        var lines = new List<SalesOrderLine>();

        foreach (var l in request.Lines)
        {
            var item = await _inventoryRepo.GetBySkuAsync(l.Sku, ct)
                       ?? throw new InvalidOperationException("SKU not found");

            if (item.AvailableQuantity < l.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            item.Reserve(l.Quantity);

            // FIX: Wrap the decimal UnitPrice in a Money object
            lines.Add(new SalesOrderLine(l.Sku, l.Quantity, new Money(l.UnitPrice)));
        }


        var order = new SalesOrder(request.BpCode, lines);

        await _salesRepo.AddAsync(order, ct);
        await _uow.SaveChangesAsync(ct);

        return new CreateSalesOrderResponse(order.Id);
    }

    public async Task<SalesOrderDetailsResponse?> GetOrderAsync(
        GetSalesOrderRequest request,
        CancellationToken ct)
    {
        var order = await _salesRepo.GetByIdAsync(request.OrderId, ct);
        if (order is null) return null;

        var dto = new SalesOrderDto(order.Id, order.BpCode, order.TotalAmount);

        return new SalesOrderDetailsResponse(dto);
    }
}