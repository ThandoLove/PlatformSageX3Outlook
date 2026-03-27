
// CODE START: IOrderService.cs

using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IOrderService
    {
        // CREATE
        Task<OrderDto> CreateOrderAsync(OrderDto order);

        // QUOTES
        Task CreateQuoteAsync(object model);

        // READ (LIST)
        Task<List<OpenOrderDto>> GetOpenOrdersAsync();

        // READ (DETAIL)
        Task<SalesOrderDto?> GetOrderByIdAsync(Guid id);
    }
}

// CODE END