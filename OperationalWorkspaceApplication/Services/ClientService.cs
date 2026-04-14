
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services
{
    public sealed class ClientService : IClientService
    {
        private readonly ILogger<ClientService> _logger;
        // Mock data storage for now - replace with IClientRepository later
        private readonly List<ClientDto> _mockClients = new();

        public ClientService(ILogger<ClientService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<ClientDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all clients");
            return await Task.FromResult(_mockClients);
        }

        public async Task<ClientDto?> GetByIdAsync(Guid id)
        {
            var client = _mockClients.FirstOrDefault(c => c.Id == id);
            return await Task.FromResult(client);
        }

      
        public async Task<ClientDto> CreateAsync(ClientDto dto)
        {
            var newClient = new ClientDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email, // Inherited from CustomerDto
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                // FIX: Changed ClientCode to CustomerId
                CustomerId = dto.CustomerId,
                Status = dto.Status,
                Balance = dto.Balance,
                CreditLimit = dto.CreditLimit
                // REMOVED: CreatedAt (Since it's not in your new ClientDto definition)
            };

            _mockClients.Add(newClient);
            _logger.LogInformation("Created new client: {Name}", newClient.Name);
            return await Task.FromResult(newClient);
        }



        public async Task<bool> UpdateAsync(ClientDto dto)
        {
            var index = _mockClients.FindIndex(c => c.Id == dto.Id);
            if (index == -1) return false;

            _mockClients[index] = dto;
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var client = _mockClients.FirstOrDefault(c => c.Id == id);
            if (client == null) return false;

            _mockClients.Remove(client);
            return await Task.FromResult(true);
        }
    }
}
