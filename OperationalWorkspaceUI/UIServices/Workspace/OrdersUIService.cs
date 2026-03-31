using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// Add this to reference the model defined in your Razor page
using static OperationalWorkspaceUI.Components.Pages.Forms.CreateQuoteForm;

namespace OperationalWorkspaceUI.UIServices.Workspace
{
    public class OrdersUIService
    {
        private readonly List<string> _orders = new();

        public Task<List<string>> GetOrdersAsync()
        {
            return Task.FromResult(_orders);
        }

        public Task AddOrderAsync(string order)
        {
            _orders.Add(order);
            return Task.CompletedTask;
        }

        public async Task LoadQuotesAsync() { /* logic */ await Task.CompletedTask; }
        public async Task ApproveQuoteAsync(Guid id) { /* logic */ await Task.CompletedTask; }
        public async Task ConvertToOrderAsync(Guid id) { /* logic */ await Task.CompletedTask; }

        public async Task<bool> CreateOrderAsync(CreateOrderDto order)
        {
            // API logic
            return await Task.FromResult(true);
        }

        // ADD THIS METHOD:
        public async Task<bool> CreateQuoteAsync(CreateQuoteModel model)
        {
            // TODO: Map 'CreateQuoteModel' to your API request DTO
            // Example: var result = await _http.PostAsJsonAsync("api/quotes", model);

            await Task.Delay(100); // Simulate network
            return true;
        }
    }
}
