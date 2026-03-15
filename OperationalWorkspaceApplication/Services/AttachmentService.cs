using System.Linq; // <--- ADD THIS LINE
using OperationalWorkspaceApplication.Abstractions;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Mappers;

namespace OperationalWorkspaceApplication.Services;

public sealed class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public AttachmentService(
        IAttachmentRepository repository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result<UploadAttachmentResponse>> UploadAsync(
        UploadAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        var attachment = new Attachment(
            request.OwnerType,
            request.OwnerId,
            request.FileName,
            request.ContentType,
            request.FileSize,
            request.StoragePath,
            _clock.UtcNow);

        await _repository.AddAsync(attachment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UploadAttachmentResponse>.Success(
            new UploadAttachmentResponse(attachment.Id));
    }

    public async Task<Result<bool>> DeleteAsync(
        DeleteAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        var attachment = await _repository.GetAsync(
            request.AttachmentId,
            cancellationToken);

        if (attachment is null)
            return Result<bool>.Failure("Attachment not found");

        _repository.Remove(attachment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<AttachmentListResponse>> GetAsync(
    GetAttachmentsRequest request,
    CancellationToken cancellationToken)
    {
        var items = await _repository.GetByOwnerAsync(
            request.OwnerType,
            request.OwnerId,
            cancellationToken);

        // FIX: Using Cast<T> ensures LINQ uses your Domain Entity, not the Mail object
        var dtos = items
            .Cast<OperationalWorkspace.Domain.Entities.Attachment>()
            .Select(x => ApplicationMapper.ToAttachmentDto(x))
            .ToList();

        return Result<AttachmentListResponse>.Success(
            new AttachmentListResponse(dtos));
    }

}
