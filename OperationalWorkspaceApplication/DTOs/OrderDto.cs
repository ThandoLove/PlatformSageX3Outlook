namespace OperationalWorkspaceApplication.DTOs;

/// <summary>
/// LEGACY DTO - DO NOT USE FOR NEW FEATURES.
/// Replaced by OpenOrderDto.
/// </summary>
public class OrderDto
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public Guid ClientId { get; set; }

    public DateTime OrderDate { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Open";

    public List<SalesOrderLineDto> Lines { get; set; } = new();
}