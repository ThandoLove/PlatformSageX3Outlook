using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.Exceptions;
using OperationalWorkspace.Domain.ValueObjects;

namespace OperationalWorkspaceDomain.DomainServices;

public class BusinessPartnerRules
{
    private const decimal HighRiskOverdueThreshold = 0.20m;

    public void EnsurePartnerIsEligibleForOrder(BusinessPartner partner, Money requestedOrderAmount)
    {
        if (!partner.IsActive)
        {
            // Fix: Use BusinessRuleException instead of abstract DomainException
            throw new BusinessRuleException($"Business Partner {partner.BpCode} is inactive.");
        }

        if (partner.IsOverCreditLimit(requestedOrderAmount.Amount))
        {
            throw new CreditLimitExceededException(
                partner.BpCode,
                partner.OverdueInvoices + requestedOrderAmount.Amount,
                partner.CreditLimit);
        }

        if (IsHighRisk(partner))
        {
            // Fix: Use BusinessRuleException instead of abstract DomainException
            throw new BusinessRuleException($"Partner {partner.BpCode} is marked High Risk due to overdue ratio.");
        }
    }

    public bool IsHighRisk(BusinessPartner partner)
    {
        if (partner.CreditLimit == 0) return partner.OverdueInvoices > 0;
        return (partner.OverdueInvoices / partner.CreditLimit) > HighRiskOverdueThreshold;
    }

    public Money CalculateAvailableCredit(BusinessPartner partner)
    {
        var available = partner.CreditLimit - partner.OverdueInvoices;
        return new Money(Math.Max(0, available));
    }
}
