

namespace OperationalWorkspaceApplication.DTOs
{
  

    public sealed class AgingBucketDto
    {
        // ===== Context =====
        public string BusinessPartnerCode { get; init; } = string.Empty;
        public string BusinessPartnerName { get; init; } = string.Empty;

        // ===== Currency =====
        public string CurrencyCode { get; init; } = "USD";

        // ===== Aging Buckets =====
        public decimal Current { get; init; }

        public decimal Days1To30 { get; init; }

        public decimal Days31To60 { get; init; }

        public decimal Days61To90 { get; init; }

        public decimal Days91To120 { get; init; }

        public decimal Days120Plus { get; init; }

        // ===== Totals =====
        public decimal TotalOutstanding { get; init; }

        public decimal TotalOverdue { get; init; }

        // ===== Credit Control =====
        public decimal CreditLimit { get; init; }

        public decimal CreditExposure { get; init; }

        public bool IsOverCreditLimit { get; init; }

        // ===== Metadata =====
        public DateTime CalculatedAtUtc { get; init; }
    }
}
