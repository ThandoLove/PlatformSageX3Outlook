using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System.Linq;

namespace OperationalWorkspace.Application.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IClock _clock;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IAuditLogRepository auditRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        IClock clock)
    {
        _inventoryRepository = inventoryRepository;
        _auditLogRepository = auditRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _clock = clock;
    }

    public async Task<InventoryItemDto?> GetItemAsync(Guid inventoryItemId, CancellationToken ct)
    {
        var item = await _inventoryRepository.GetByIdAsync(inventoryItemId, ct);
        if (item == null) return null;

        return new InventoryItemDto
        {
            ItemId = item.Id,
            ItemCode = item.ItemCode,
            WarehouseCode = item.WarehouseCode,
            QuantityOnHand = item.QuantityOnHand,
            QuantityReserved = item.QuantityReserved,
            QuantityAvailable = item.QuantityOnHand - item.QuantityReserved,
            UnitCost = item.UnitCost,
            LastUpdatedUtc = item.LastUpdatedUtc
        };
    }

    public async Task<IReadOnlyList<InventoryItemDto>> GetWarehouseInventoryAsync(string warehouseCode, CancellationToken ct)
    {
        var items = await _inventoryRepository.GetByWarehouseAsync(warehouseCode, ct);
        return items.Select(i => new InventoryItemDto
        {
            ItemId = i.Id,
            ItemCode = i.ItemCode,
            WarehouseCode = i.WarehouseCode,
            QuantityOnHand = i.QuantityOnHand,
            QuantityReserved = i.QuantityReserved,
            QuantityAvailable = i.QuantityOnHand - i.QuantityReserved,
            UnitCost = i.UnitCost,
            LastUpdatedUtc = i.LastUpdatedUtc
        }).ToList();
    }

    public async Task<StockAvailabilityResponse> CheckAvailabilityAsync(CheckStockRequest request, CancellationToken ct)
    {
        var item = await _inventoryRepository.GetByIdAsync(request.InventoryItemId, ct);
        if (item == null) return new StockAvailabilityResponse(false, 0);

        var available = item.QuantityOnHand - item.QuantityReserved;
        return new StockAvailabilityResponse(available >= request.Quantity, available);
    }

    public async Task<AdjustStockResponse> AdjustStockAsync(StockAdjustmentRequest request, CancellationToken ct)
    {
        try
        {
            await GetAdjustmentDetailsAsync(request, ct);
            return new AdjustStockResponse(true, "Success");
        }
        catch (Exception ex)
        {
            return new AdjustStockResponse(false, ex.Message);
        }
    }

    public async Task<StockAdjustmentDto> GetAdjustmentDetailsAsync(StockAdjustmentRequest request, CancellationToken ct)
    {
        // 1. Get the current user from your existing context service
        var currentUser = await _userContextService.GetCurrentUserAsync();

        var item = await _inventoryRepository.GetByIdAsync(request.InventoryItemId, ct);
        if (item == null) throw new InvalidOperationException("Inventory item not found");

        var previousQuantity = item.QuantityOnHand;
        item.AdjustQuantity(request.QuantityChange, request.AdjustmentType, _clock.UtcNow);

        await _inventoryRepository.UpdateAsync(item, ct);

        // 2. Log the audit entry using the user's name
        await _auditLogRepository.AddAsync(new AuditLogEntry
        {
            EventType = "Adjustment",
            Description = $"Item {item.ItemCode} adjusted by {currentUser.Name}. Change: {request.QuantityChange}",
            CreatedAt = _clock.UtcNow
        }, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return new StockAdjustmentDto
        {
            Id = Guid.NewGuid(),
            InventoryItemId = item.Id,
            ItemCode = item.ItemCode,
            QuantityBefore = previousQuantity,
            QuantityAdjusted = request.QuantityChange,
            QuantityAfter = item.QuantityOnHand,
            AdjustmentType = request.AdjustmentType,
            ReasonCode = request.ReasonCode,
            AdjustedAtUtc = _clock.UtcNow
        };
    }

    public async Task<int> CountStockAlertsAsync()
    {
        return await _inventoryRepository.GetStockAlertCountAsync();
    }
}
