
using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllAsync();
        Task<ClientDto?> GetByIdAsync(Guid id);
        Task<ClientDto> CreateAsync(ClientDto dto);
        Task<bool> UpdateAsync(ClientDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
