using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{
    public sealed record CreateSalesOrderRequest(
        string BpCode,
        List<CreateSalesOrderLineRequest> Lines);

    public sealed record CreateSalesOrderLineRequest(
        string Sku,
        int Quantity,
        decimal UnitPrice);

    public sealed record GetSalesOrderRequest(Guid OrderId);
}