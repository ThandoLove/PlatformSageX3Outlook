using System;
using System.Collections.Generic;

namespace OperationalWorkspaceApplication.DTOs
{
    // Primary constructor mirrors the usage sites which pass 8 parameters.
    public record BusinessPartnerSnapshotDto(
        string BpCode,
        string Company,
        decimal CreditLimit,
        decimal TotalOutstanding,
        int OpenOrderCount,
        int OverdueInvoiceCount,
        decimal OverdueAmount,
        decimal Balance,
        DateTime LastContactDate)
    {
        // 🚀 FIXED: Added infrastructure mapping properties needed by the context builder
        public Guid Id { get; init; } = Guid.NewGuid();
        public string AssignedToUserId { get; init; } = string.Empty;

        // Additional metadata exposed as init-only properties with defaults
        public string FullName { get; init; } = string.Empty;
        public bool IsLinkedToSage { get; init; } = false;
        public string Location { get; init; } = string.Empty;
        public string AssignedRep { get; init; } = string.Empty;
        public List<ActivityDto> Timeline { get; init; } = new();
    }
}
