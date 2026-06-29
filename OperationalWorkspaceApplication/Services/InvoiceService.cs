using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.Abstractions;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly ISageX3Client _sageClient;
    private readonly ILogger<InvoiceService> _logger;
    private readonly IClock _clock;

    public InvoiceService(
        ISageX3Client sageClient,
        ILogger<InvoiceService> logger,
        IClock clock)
    {
        _sageClient = sageClient;
        _logger = logger;
        _clock = clock;
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        // Converts the Guid identifier securely to match your string-based client interface
        return await _sageClient.GetInvoiceAsync(id.ToString());
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize)
    {
        return await _sageClient.GetInvoicesPagedAsync(page, pageSize);
    }

    public async Task<int> CountOverdueInvoicesAsync()
    {
        return await _sageClient.GetOverdueInvoicesCountAsync();
    }

    public async Task<int> CountInvoicesGeneratedAsync()
    {
        return await _sageClient.GetTotalGeneratedInvoicesCountAsync();
    }

    public async Task<int> CountInvoicesDueAsync(string userId)
    {
        return await _sageClient.GetUserInvoicesDueCountAsync(userId);
    }

    public async Task<decimal> GetOutstandingInvoiceValueAsync()
    {
        return await _sageClient.GetOutstandingInvoiceValueAsync();
    }

    public async Task<decimal> GetUserOutstandingInvoiceValueAsync(string userId)
    {
        return await _sageClient.GetUserOutstandingInvoiceValueAsync(userId);
    }

    public async Task<decimal> GetCurrentMonthInvoiceValueAsync()
    {
        return await _sageClient.GetCurrentMonthInvoiceValueAsync();
    }

    public async Task<decimal> GetUserCurrentMonthInvoiceValueAsync(string userId)
    {
        return await _sageClient.GetUserCurrentMonthInvoiceValueAsync(userId);
    }
}
