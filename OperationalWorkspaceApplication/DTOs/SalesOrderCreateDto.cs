namespace OperationalWorkspaceApplication.DTOs;

/// <summary>
/// Used when creating a new Sage X3 sales order.
/// </summary>
public class SalesOrderCreateDto
{
    public string CustomerCode { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public string CustomerRef { get; set; } = string.Empty;

    public List<SalesOrderLineCreateDto> Lines { get; set; } = new();
}

public class SalesOrderLineCreateDto
{
    public string ProductCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; } = 1;

    public decimal UnitPrice { get; set; } = 0;
}