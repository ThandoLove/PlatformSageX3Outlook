
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;

namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface ISalesService
{
    Task<CreateSalesOrderResponse> CreateOrderAsync(
        CreateSalesOrderRequest request,
        CancellationToken cancellationToken);

    Task<SalesOrderDetailsResponse?> GetOrderAsync(
        GetSalesOrderRequest request,
        CancellationToken cancellationToken);
}