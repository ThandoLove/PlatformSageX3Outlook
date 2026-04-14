namespace OperationalWorkspaceApplication.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; } // Ensure Id is present for Navigation
        public string OrderNumber { get; set; } = string.Empty; // ADD THIS
        public Guid ClientId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Open"; // Grid expects Status

        public List<SalesOrderLineDto> Lines { get; set; } = new();
    }
}
