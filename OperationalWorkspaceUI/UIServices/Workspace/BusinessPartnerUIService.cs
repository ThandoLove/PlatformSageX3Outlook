using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceUI.State;

namespace OperationalWorkspaceUI.UIServices.Workspace
{
    public class BusinessPartnerUIService
    {
        private readonly WorkspaceState _state;

        public BusinessPartnerUIService(WorkspaceState state)
        {
            _state = state;
        }

        public async Task LoadClientsAsync()
        {
            // Standardize on ClientDto (PascalCase)
            if (_state.Clients == null)
            {
                _state.Clients = new List<ClientDto>();
            }

            // Example: _state.Clients = await _http.GetFromJsonAsync<List<ClientDto>>("api/partners") ?? new();

            await Task.CompletedTask;
        }

        // FIX: Ensure return type matches the WorkspaceState.Clients type exactly
        public Task<List<ClientDto>> GetPartnersAsync()
        {
            return Task.FromResult(_state.Clients ?? new List<ClientDto>());
        }

        // FIX: Added missing method called by CreateClient.razor
        public async Task<bool> CreateClientAsync(ClientDto partner)
        {
            try
            {
                // Simulate API POST call here
                _state.Clients?.Add(partner);
                await Task.CompletedTask;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task AddPartnerAsync(ClientDto partner)
        {
            _state.Clients?.Add(partner);
            return Task.CompletedTask;
        }
    }
}
