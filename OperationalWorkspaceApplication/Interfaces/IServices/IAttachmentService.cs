using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using OperationalWorkspaceApplication.IServices;
using OperationalWorkspaceApplication.DTOs;


namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IAttachmentService
    {
        Task<Result<UploadAttachmentResponse>> UploadAsync(
            UploadAttachmentRequest request,
            CancellationToken cancellationToken);

        Task<Result<bool>> DeleteAsync(
            DeleteAttachmentRequest request,
            CancellationToken cancellationToken);

        Task<Result<AttachmentListResponse>> GetAsync(
            GetAttachmentsRequest request,
            CancellationToken cancellationToken);
        Task<List<AttachmentDto>> GetRecentAttachmentsAsync(string userId);
    }
}
