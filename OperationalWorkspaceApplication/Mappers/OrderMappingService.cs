
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.Mappers;

/// <summary>
/// Central mapping layer between legacy OrderDto and modern OpenOrderDto.
/// </summary>
public static class OrderMappingService
{
    public static OpenOrderDto ToOpenOrder(OrderDto dto)
    {
        return new OpenOrderDto
        {
            Id = dto.Id,
            OrderNumber = dto.OrderNumber,
            OrderDate = dto.OrderDate,
            TotalAmount = dto.TotalAmount,
            Status = dto.Status,
            ClientName = "Unknown Client"
        };
    }

    public static OrderDto ToLegacy(OpenOrderDto dto)
    {
        return new OrderDto
        {
            Id = dto.Id,
            OrderNumber = dto.OrderNumber,
            OrderDate = dto.OrderDate ?? DateTime.UtcNow,
            TotalAmount = dto.TotalAmount,
            Status = dto.Status,
            ClientId = Guid.Empty
        };
    }
}