using Microsoft.EntityFrameworkCore.Infrastructure;
using OperationalWorkspaceApplication.DTOs;


namespace OperationalWorkspaceApplication.Responses;


    
    public sealed record CreateSalesOrderResponse(Guid OrderId);
    public sealed record SalesOrderDetailsResponse(SalesOrderDto Order);

    

