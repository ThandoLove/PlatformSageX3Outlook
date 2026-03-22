
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;

namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface ISalesService
{
    Task<int> CountOpenOrdersAsync(string userId);
    Task<int> CountPendingDeliveriesAsync(string userId);
    Task<int> CountTotalOrdersAsync();
    Task<CreateSalesOrderResponse> CreateOrderAsync(
        CreateSalesOrderRequest request,
        CancellationToken cancellationToken);

    Task<SalesOrderDetailsResponse?> GetOrderAsync(
        GetSalesOrderRequest request,
        CancellationToken cancellationToken);
}