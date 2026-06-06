using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.IServices;
using OperationalWorkspaceApplication.Mappers;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services;

public sealed class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;

    // 🚨 CONFIGURATION TOGGLE: Set to false for local dev mock data, true for live Sage servers
    private readonly bool _isSageConnected;

    public AttachmentService(
        IAttachmentRepository repository,
        IUnitOfWork unitOfWork,
        IClock clock,
        IMemoryCache cache,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _cache = cache;
        _httpClient = httpClient;

        // Reads 'UseMockData' from the SageX3 section, then flips it so _isSageConnected is true if mock data is false
        var useMockData = configuration.GetValue<bool>("SageX3:UseMockData");
        _isSageConnected = !useMockData;
    }


    // 👇 ENTERPRISE METHOD 1: DUAL MOCK / REAL SAGE BINARY STREAMER
    public async Task<byte[]> GetAttachmentBinaryStreamAsync(Guid id, string userContext)
    {
        string cacheKey = $"attachment_stream_{id}";

        // 1. PREVIEW CACHING LAYER INTERACTION
        // FIX: Explicitly check type matching and handle cache retrieval cleanly
        if (_cache.TryGetValue(cacheKey, out object? cachedObject) && cachedObject is byte[] cachedBytes)
        {
            await LogAttachmentAuditTrailAsync(id, "Served from Memory Cache", userContext);
            return cachedBytes;
        }

        byte[] attachmentPayload;

        // 2. DUAL FLOW SWITCH: DEVELOPMENT MOCK VS LIVE SAGE ERP
        if (_isSageConnected)
        {
            // PROPER SAGE X3 DOCUMENT LINKING PIPELINE
            var sageUrl = $"https://your-sage-x3-node:8124/api1/x3/erp/SEED/CUMDOCS('{id}')";
            var request = new HttpRequestMessage(HttpMethod.Get, sageUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "YXV0aC1rZXktZ2VuZXJhdGVk");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            attachmentPayload = await response.Content.ReadAsByteArrayAsync();
        }
        else
        {
            // DEVELOPMENT STREAMING ALLOWANCE
            attachmentPayload = Convert.FromBase64String("JVBERi0xLjQKMSAwIG9iajw8L1R5cGUvQ2F0YWxvZy9QYWdlcyAyIDAgUj4+ZW5kb2JqMiAwIG9iajw8L1R5cGUvUGFnZXMvQ291bnQgMS9LaWRzWzMgMCBSXT4+ZW5kb2JqMyAwIG9iajw8L1R5cGUvUGFnZS9QYXJlbnQgMiAwIFIvTWVkaWFCb3hbMCAwIDU5NSA4NDJdL0NvbnRlbnRzIDQgMCBSPj5lbmRvYmo0IDAgb2JqPDwvTGVuZ3RoIDU5Pj5zdHJlYW0KQlQgL0YxIDEyIFRmIDcwIDcwMCBUZCAoRW50ZXJwcmlzZSBTYWdlIFgzIE1vY2sgUERGIFN0cmVhbSkgVGogRVQKZW5kc3RyZWFtZW5kb2JqeHJlZgowIDUKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDA5IDAwMDAwIG4gCjAwMDAwMDAwNTYgMDAwMDAgb gogglesowMDAwMDAwMTExIDAwMDAwIG4gCjAwMDAwMDAyMDQgMDAwMDAgbIAp0cmFpbGVyPDwvU2l6ZSA1L1Jvb3QgMSAwIFI+PnN0YXJ0eHJlZgogMjk4CiUlRU9G");
        }

        // Save binary stream into Memory Cache for 5 minutes
        var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        _cache.Set(cacheKey, attachmentPayload, cacheOptions);

        await LogAttachmentAuditTrailAsync(id, "Stream Binary Pulled and Cached", userContext);
        return attachmentPayload;
    }

    // 👇 ENTERPRISE METHOD 2: EXPOSE SECURE DOCUMENT CONTENT TYPE
    public async Task<string> GetAttachmentMimeTypeAsync(Guid id)
    {
        return "application/pdf";
    }

    // 👇 ENTERPRISE METHOD 3: AUDIT TRAIL LOGGING PER ATTACHMENT VIEW
    public async Task LogAttachmentAuditTrailAsync(Guid id, string action, string userContext)
    {
        string logLine = $"[AUDIT] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | Attachment ID: {id} | Action: {action} | User: {userContext}";
        System.Diagnostics.Debug.WriteLine(logLine);
        await Task.CompletedTask;
    }

    public async Task<Result<UploadAttachmentResponse>> UploadAsync(
        UploadAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        // FIX: Added guard patterns to guarantee fallback options for null string fields
        var attachment = new Attachment(
            request.OwnerType ?? "UnknownOwnerType",
            request.OwnerId ?? "UnknownOwnerId",
            request.FileName ?? "unnamed_file",
            request.ContentType ?? "application/octet-stream",
            request.FileSize,
            request.StoragePath ?? string.Empty,
            _clock.UtcNow,
            request.Source ?? "SageX3Outlook"
        );

        attachment.EntityId = request.EntityId;

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
            request.OwnerType ?? string.Empty,
            request.OwnerId ?? string.Empty,
            cancellationToken);

        // FIX: Filter out any mapping loops that might return null objects
        var dtos = items
            .Where(x => x is not null)
            .Select(x => ApplicationMapper.ToAttachmentDto(x))
            .ToList();

        return Result<AttachmentListResponse>.Success(
            new AttachmentListResponse(dtos));
    }

    public async Task<List<AttachmentDto>> GetRecentAttachmentsAsync(string userId)
    {
        var attachments = await _repository.GetRecentByUserIdAsync(userId ?? string.Empty);

        // FIX: Filter out null instances defensively before mapping DTO values
        return attachments
            .Where(x => x is not null)
            .Select(x => ApplicationMapper.ToAttachmentDto(x))
            .ToList();
    }
}
