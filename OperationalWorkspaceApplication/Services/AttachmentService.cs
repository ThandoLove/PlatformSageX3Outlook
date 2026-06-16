using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.IServices;
using OperationalWorkspaceApplication.Mappers;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System.Net.Http.Headers;


namespace OperationalWorkspaceApplication.Services;

public sealed class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IAttachmentProvider _provider;
    private readonly bool _useMock;
    private readonly bool _autoFallback;

    public AttachmentService(
        IAttachmentRepository repository,
        IUnitOfWork unitOfWork,
        IClock clock,
        IMemoryCache cache,
        HttpClient httpClient,
        IConfiguration configuration,
        IAttachmentProvider provider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _cache = cache;
        _httpClient = httpClient;
        _configuration = configuration;
        _provider = provider;
        _useMock = configuration.GetValue<bool>("SageX3:UseMockData");
        _autoFallback = configuration.GetValue<bool>("SageX3:AutoFallbackToMock");
    }
    private async Task<bool> IsSageAvailable()
    {
        try
        {
            var url = _configuration["SageX3:BaseUrl"];
            if (string.IsNullOrWhiteSpace(url)) return false;
            var response = await _httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> UseMockMode()
    {
        if (_useMock) return true;
        bool sageAvailable = await IsSageAvailable();
        if (sageAvailable) return false;
        return _autoFallback;
    }

    public async Task<byte[]> GetAttachmentBinaryStreamAsync(Guid id, string userContext)
    {
        string cacheKey = $"attachment_stream_{id}";
        if (_cache.TryGetValue(cacheKey, out object? cachedObject) && cachedObject is byte[] cachedBytes)
        {
            await LogAttachmentAuditTrailAsync(id, "Served from Cache", userContext);
            return cachedBytes;
        }

        byte[] attachmentPayload;
        bool useMock = await UseMockMode();

        if (!useMock)
        {
            string baseUrl = _configuration["SageX3:RestBaseUrl"] ?? "";
            string sageUrl = $"{baseUrl}/CUMDOCS('{id}')";
            var request = new HttpRequestMessage(HttpMethod.Get, sageUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "YXV0aC1rZXktZ2VuZXJhdGVk");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            attachmentPayload = await response.Content.ReadAsByteArrayAsync();
        }
        else
        {
            attachmentPayload = System.Text.Encoding.UTF8.GetBytes("Mock Attachment");
        }

        _cache.Set(cacheKey, attachmentPayload, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        await LogAttachmentAuditTrailAsync(id, "Attachment Cached", userContext);
        return attachmentPayload;
    }

    public async Task<string> GetAttachmentMimeTypeAsync(Guid id)
    {
        return await Task.FromResult("application/pdf");
    }

    public async Task LogAttachmentAuditTrailAsync(Guid id, string action, string userContext)
    {
        string logLine = $"[AUDIT] {DateTime.UtcNow} | {id} | {action} | {userContext}";
        System.Diagnostics.Debug.WriteLine(logLine);
        await Task.CompletedTask;
    }
    public async Task<Result<UploadAttachmentResponse>> UploadAsync(UploadAttachmentRequest request, CancellationToken cancellationToken)
    {
        var attachment = new Attachment(
            request.OwnerType ?? "UnknownOwnerType",
            request.OwnerId ?? "UnknownOwnerId",
            request.FileName ?? "unnamed_file",
            request.ContentType ?? "application/octet-stream",
            request.FileSize,
            request.StoragePath ?? "",
            _clock.UtcNow,
            request.Source ?? "SageX3Outlook"
        );

        attachment.EntityId = request.EntityId;
        await _repository.AddAsync(attachment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<UploadAttachmentResponse>.Success(new UploadAttachmentResponse(attachment.Id));
    }

    public async Task<Result<bool>> DeleteAsync(DeleteAttachmentRequest request, CancellationToken cancellationToken)
    {
        var attachment = await _repository.GetAsync(request.AttachmentId, cancellationToken);
        if (attachment is null) return Result<bool>.Failure("Attachment not found");

        _repository.Remove(attachment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<AttachmentListResponse>> GetAsync(
        GetAttachmentsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var attachments = await _provider.GetAttachmentsAsync(
                request.OwnerType ?? "",
                request.OwnerId ?? "",
                cancellationToken);

            return Result<AttachmentListResponse>.Success(
                new AttachmentListResponse(attachments));
        }
        catch (Exception ex)
        {
            return Result<AttachmentListResponse>.Failure(ex.Message);
        }
    }

    public async Task<List<AttachmentDto>> GetRecentAttachmentsAsync(string userId)
    {
        var attachments = await _repository.GetRecentByUserIdAsync(userId ?? "");
        return attachments
            .Where(x => x is not null)
            .Select(ApplicationMapper.ToAttachmentDto)
            .ToList();
    }
}
