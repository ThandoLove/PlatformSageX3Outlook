using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceUI.State;

namespace OperationalWorkspaceUI.UIServices.Workspace
{
    public class BusinessPartnerUIService
    {
        private readonly WorkspaceState _state;
        private readonly HttpClient _http; // 🔥 Added HttpClient parameter

        public BusinessPartnerUIService(WorkspaceState state, HttpClient http)
        {
            _state = state;
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task LoadClientsAsync()
        {
            if (_state.Clients == null)
            {
                _state.Clients = new List<ClientDto>();
            }
            await Task.CompletedTask;
        }

        public Task<List<ClientDto>> GetPartnersAsync()
        {
            return Task.FromResult(_state.Clients ?? new List<ClientDto>());
        }

        // 🔥 UPGRADED TO TALK TO THE BACKEND CONTROLLER REAL-TIME
        public async Task<bool> CreateClientAsync(ClientDto partner)
        {
            try
            {
                // Dispatches the clean UI DTO down into your actual backend API endpoints
                var response = await _http.PostAsJsonAsync("api/businesspartner/create", partner);

                if (response.IsSuccessStatusCode)
                {
                    _state.Clients?.Add(partner);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UI Service transmission failed: {ex.Message}");
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
