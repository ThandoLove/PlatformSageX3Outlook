using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; } // Added
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal OutstandingAmount { get; set; } // Added to match service
    public decimal Balance { get; set; }
    public bool IsOverdue { get; set; }
    public decimal Amount { get; internal set; }
    
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
 
    public string Currency { get; init; } = "USD";
    public DateTime IssueDate { get; init; }

    public string Status { get; init; } = "Unpaid";
}

