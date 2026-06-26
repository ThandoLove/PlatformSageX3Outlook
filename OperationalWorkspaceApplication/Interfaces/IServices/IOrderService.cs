
// CODE START: IOrderService.cs

using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IOrderService
    {
        // CREATE
       

     

        // READ (LIST)
        Task<List<OpenOrderDto>> GetOpenOrdersAsync();

        // READ (DETAIL)
        Task<SalesOrderDto?> GetOrderByIdAsync(Guid id);
    }
}

// CODE END