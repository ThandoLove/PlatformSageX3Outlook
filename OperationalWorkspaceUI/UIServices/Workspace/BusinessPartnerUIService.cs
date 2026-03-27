using System.Collections.Generic;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs; // Ensure this matches your DTO location
using OperationalWorkspaceUI.State; // Ensure this matches your WorkspaceState location

namespace OperationalWorkspaceUI.UIServices.Workspace
{
    public class BusinessPartnerUIService
    {
        private readonly WorkspaceState _state;

        // Inject WorkspaceState to manage the global client list
        public BusinessPartnerUIService(WorkspaceState state)
        {
            _state = state;
        }

        // FIXED: Renamed to match your UI's call and uses DTOs
        public async Task LoadClientsAsync()
        {
            // Simulate API call - Replace with: 
            // _state.Clients = await _http.GetFromJsonAsync<List<ClientDTO>>("api/partners");

            if (_state.Clients == null || !_state.Clients.Any())
            {
                _state.Clients = new List<ClientDTO>();
            }

            await Task.CompletedTask;
        }

        public Task<List<ClientDTO>> GetPartnersAsync()
        {
            return Task.FromResult(_state.Clients ?? new List<ClientDTO>());
        }

        public Task AddPartnerAsync(ClientDTO partner)
        {
            _state.Clients?.Add(partner);
            return Task.CompletedTask;
        }
    }
}
