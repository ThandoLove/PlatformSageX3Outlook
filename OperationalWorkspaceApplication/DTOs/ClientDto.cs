
using System;

namespace OperationalWorkspaceApplication.DTOs
{
    public class ClientDto
    {
        public Guid Id { get; set; }

        // BASIC INFO
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // ADDRESS
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        // BUSINESS DATA
        public string ClientCode { get; set; } = string.Empty;
        public string Status { get; set; } = "Active"; // Active / Inactive

        // ERP specific
        public string Category { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; }

        // FINANCIAL SUMMARY (UI-friendly)
        public decimal TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // AUDIT
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

// CODE END