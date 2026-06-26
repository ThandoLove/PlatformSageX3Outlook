using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services
{
    public class OrderService : IOrderService
    {
        // Kept for local reference/in-memory lookup simulation
        private static readonly List<SalesOrderDto> _orders = new();

        // =========================
        // GET LIST (UI GRID)
        // =========================
        public async Task<List<OpenOrderDto>> GetOpenOrdersAsync()
        {
            return _orders.Select(o => new OpenOrderDto
            {
                // Preserved explicit ID mapping for page routing
                Id = o.Id,
                OrderNumber = o.OrderNumber,

                // FIX: Conversion from nullable SalesOrderDto.OrderDate to non-nullable OpenOrderDto.OrderDate
                OrderDate = o.OrderDate ?? DateTime.Now,

                TotalAmount = o.TotalAmount,
                Status = o.OrderStatus,
                ClientName = o.BusinessPartnerName ?? "Unknown Client"
            }).ToList();
        }

        // =========================
        // GET DETAILS
        // =========================
        public async Task<SalesOrderDto?> GetOrderByIdAsync(Guid id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }
    }
}
