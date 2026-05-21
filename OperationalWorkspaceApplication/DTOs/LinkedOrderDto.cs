using System;

namespace OperationalWorkspaceApplication.DTOs
{
    /// <summary>
    /// Supplementary tracking DTO mapping open order visibility properties calculated by matching logic loops.
    /// </summary>
    public class LinkedOpenOrderDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        // Retained for absolute forward compatibility with your context calculations
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public double MatchConfidence { get; set; }
    }
}
