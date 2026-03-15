using OperationalWorkspaceApplication.DTOs;
using System.Threading.Channels;


namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IActivityService
    {
        Task<ActivityDto?> GetByIdAsync(Guid id);
        Task<ActivityDto> CreateAsync(CreateActivityDto dto, string userEmail);
       
       
        // FIX: Changed from Task<object> to Task<IEnumerable<ActivityDto>>
        // This allows the Controller and Service to handle the collection properly
        Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId);

    }

}
