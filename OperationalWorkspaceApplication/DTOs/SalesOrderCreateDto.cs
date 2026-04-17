namespace OperationalWorkspaceApplication.DTOs;

public class SalesOrderCreateDto
{
    public string CustomerCode { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public string CustomerRef { get; set; } = string.Empty;
    public List<SalesOrderLineCreateDto> Lines { get; set; } = new();
}

public class SalesOrderLineCreateDto
{
    // Changed from ItemCode to ProductCode to match your HTML @bind-Value
    public string ProductCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; } = 0;
}
