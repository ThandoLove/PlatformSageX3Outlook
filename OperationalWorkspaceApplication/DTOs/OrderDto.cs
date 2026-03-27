

using System;

namespace OperationalWorkspaceApplication.DTOs
{
    public class OrderDto
    {
        public Guid ClientId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
    }
}

// CODE END