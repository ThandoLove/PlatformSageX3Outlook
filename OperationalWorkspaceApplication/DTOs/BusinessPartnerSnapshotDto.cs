

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
        DateTime LastContactDate)
    {
        // Additional metadata exposed as init-only properties with defaults
        public string FullName { get; init; } = string.Empty;
        public bool IsLinkedToSage { get; init; } = false;
        public string Location { get; init; } = string.Empty;
        public string AssignedRep { get; init; } = string.Empty;
        public List<ActivityDto> Timeline { get; init; } = new();
    }
}