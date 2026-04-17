using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{
    // Update this to match the data your form provides
    public sealed record CreateSalesOrderRequest(
        string BpCode,
        string CustomerRef,
        decimal Quantity,
        List<CreateSalesOrderLineRequest> Lines);

    public sealed record CreateSalesOrderLineRequest(
        string Sku,
        decimal Quantity, // Changed to decimal to match Sage X3 and DTOs
        decimal UnitPrice);

    public sealed record GetSalesOrderRequest(Guid OrderId);
}
