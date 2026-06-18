using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.ValueObjects;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services;

public sealed class SalesService : ISalesService
{
    private readonly ISalesOrderRepository _salesRepo;
    private readonly IUnitOfWork _uow;

    public SalesService(
        ISalesOrderRepository salesRepo,
        IUnitOfWork uow)
    {
        _salesRepo = salesRepo;
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

        // 🚀 FIXED: Local stock checks are removed to map directly to standard sales entries
        foreach (var l in request.Lines)
        {
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
