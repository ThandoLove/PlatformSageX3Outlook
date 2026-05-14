using OperationalWorkspaceApplication.DTOs;


namespace OperationalWorkspaceApplication.Interfaces.IServices;
public interface ISystemHealthService
{
    Task<AdminSystemHealthDto> GetSystemHealthAsync();
}
