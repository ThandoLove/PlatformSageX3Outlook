
   using System.Collections.Generic;
   using System.Threading.Tasks;

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
        }
    }