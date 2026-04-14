using FluentValidation;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceShared.Validators;

public class StockAdjustmentValidator : AbstractValidator<StockAdjustmentDto>
{
    public StockAdjustmentValidator()
    {
        // 1. Identification
        RuleFor(x => x.ItemCode)
            .NotEmpty().WithMessage("Item code is required.");

        RuleFor(x => x.WarehouseCode)
            .NotEmpty().WithMessage("Warehouse code is required for ERP sync.");

        // 2. Logic: The core adjustment amount
        // In your DTO, this is 'QuantityAdjusted'
        RuleFor(x => x.QuantityAdjusted)
            .NotEqual(0).WithMessage("Adjustment quantity cannot be zero.")
            .Must(x => x < 1000000).WithMessage("Adjustment exceeds maximum allowed threshold.");

        // 3. Reason & Notes
        // In your DTO, these are 'ReasonCode' and 'Notes'
        RuleFor(x => x.ReasonCode)
            .NotEmpty().WithMessage("A reason code must be selected.");

        RuleFor(x => x.AdjustmentType)
            .NotEmpty().WithMessage("Adjustment type (e.g. Damage, Correction) is required.");

        // 4. Financials
        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("Unit cost cannot be negative.");
    }
}
