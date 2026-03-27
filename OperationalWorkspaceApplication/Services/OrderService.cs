using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;


// CODE START: OrderService.cs

namespace OperationalWorkspaceApplication.Services
{
    public class OrderService : IOrderService
    {
        // ⚠️ Replace with repository later
        private static readonly List<SalesOrderDto> _orders = new();

        // =========================
        // CREATE ORDER
        // =========================
        public async Task<OrderDto> CreateOrderAsync(OrderDto order)
        {
            var salesOrder = new SalesOrderDto
            {
                Id = Guid.NewGuid(),
                OrderNumber = GenerateOrderNumber(),
                OrderDate = order.OrderDate,

                BusinessPartnerId = order.ClientId,
                BusinessPartnerName = "Auto-Generated",

                SubTotal = order.TotalAmount,
                TaxAmount = 0,
                DiscountAmount = 0,
                TotalAmount = order.TotalAmount,

                OrderStatus = "Draft",
                PaymentStatus = "Unpaid",
                FulfillmentStatus = "NotStarted",

                CreatedAtUtc = DateTime.UtcNow
            };

            _orders.Add(salesOrder);

            return new OrderDto
            {
                ClientId = order.ClientId,
                OrderDate = order.OrderDate,
                Description = order.Description,
                TotalAmount = order.TotalAmount
            };
        }

        // =========================
        // CREATE QUOTE
        // =========================
        public async Task CreateQuoteAsync(object model)
        {
            // TODO: Replace with proper QuoteDTO
            await Task.Delay(200);

            var quote = new SalesOrderDto
            {
                Id = Guid.NewGuid(),
                OrderNumber = "Q-" + GenerateOrderNumber(),
                OrderDate = DateTime.UtcNow,

                BusinessPartnerName = "Quote Client",

                TotalAmount = 0,
                OrderStatus = "Draft",
                PaymentStatus = "Unpaid",
                FulfillmentStatus = "NotStarted",

                CreatedAtUtc = DateTime.UtcNow
            };

            _orders.Add(quote);
        }

        // =========================
        // GET LIST (UI GRID)
        // =========================
        public async Task<List<OpenOrderDto>> GetOpenOrdersAsync()
        {
            return _orders.Select(o => new OpenOrderDto
            {
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.OrderStatus
            }).ToList();
        }

        // =========================
        // GET DETAILS
        // =========================
        public async Task<SalesOrderDto?> GetOrderByIdAsync(Guid id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }

        // =========================
        // HELPER
        // =========================
        private string GenerateOrderNumber()
        {
            return $"SO-{DateTime.UtcNow.Ticks.ToString()[^6..]}";
        }
    }
}

// CODE END