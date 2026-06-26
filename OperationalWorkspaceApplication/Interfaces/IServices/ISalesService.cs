
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;

namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface ISalesService
{
    Task<int> CountOpenOrdersAsync(string userId);
    Task<int> CountPendingDeliveriesAsync(string userId);
    Task<int> CountTotalOrdersAsync();
  

    Task<SalesOrderDetailsResponse?> GetOrderAsync(
        GetSalesOrderRequest request,
        CancellationToken cancellationToken);

    
}