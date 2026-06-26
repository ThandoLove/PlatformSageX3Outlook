using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.Workspace
{
    public class OrdersUIService
    {
        private readonly List<string> _orders = new();

        // =================================
        // GET LIST (UI GRID REFERENCE)
        // =================================
        public Task<List<string>> GetOrdersAsync()
        {
            return Task.FromResult(_orders);
        }

        // =================================
        // READ-ONLY SAGE REFERENCE TRACKING
        // =================================
        public async Task LoadQuotesAsync()
        {
            /* Read-only logic to fetch live quotes from Sage X3 APIs */
            await Task.CompletedTask;
        }
    }
}
