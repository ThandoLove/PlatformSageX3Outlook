using System;
using System.Collections.Generic;
using System.Text;

// CODE START: CreateOrderDto.cs

namespace OperationalWorkspaceApplication.DTOs;

public class CreateOrderDto
{
    public string OrderNumber { get; set; } = string.Empty;

    public Guid ClientId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }
}

// CODE END