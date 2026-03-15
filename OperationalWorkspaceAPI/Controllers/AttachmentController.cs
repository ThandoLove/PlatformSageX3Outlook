using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using System.IO;

namespace OperationalWorkspaceAPI.Controllers;

public sealed class AttachmentController : ApiController
{
    private readonly IAttachmentService _service;
    private readonly string[] _permittedExtensions = { ".pdf", ".jpg", ".png", ".docx" };
    private const long _maxFileSize = 10 * 1024 * 1024;

    public AttachmentController(IAttachmentService service) => _service = service;

    [HttpPost("upload")]
    [RequestSizeLimit(_maxFileSize)]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string ownerId, [FromQuery] string ownerType, CancellationToken ct)
    {
        if (file == null || file.Length == 0) return Failure("File is empty");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !_permittedExtensions.Contains(ext))
            return Failure("Unsupported file type");

        // Your record requires a StoragePath string. 
        // This logic assumes the service or a helper handles the physical move later.
        var tempPath = Path.Combine(Path.GetTempPath(), file.FileName);

        // Match the 6-parameter constructor of UploadAttachmentRequest
        var request = new UploadAttachmentRequest(
            ownerType,
            ownerId,
            file.FileName,
            file.ContentType,
            file.Length,
            tempPath
        );

        var result = await _service.UploadAsync(request, ct);

        return result.IsSuccess ? Success(result.Value) : Failure(message: result.Error);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAttachments([FromQuery] string ownerType, [FromQuery] string ownerId, CancellationToken ct)
    {
        // Fix: Use GetAsync and the GetAttachmentsRequest record
        var request = new GetAttachmentsRequest(ownerType, ownerId);
        var result = await _service.GetAsync(request, ct);

        return result.IsSuccess ? Success(result.Value) : Failure(result.Error);
    }
}
