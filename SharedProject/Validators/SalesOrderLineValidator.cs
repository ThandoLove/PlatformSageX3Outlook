using FluentValidation;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceShared.Validators;

public class SalesOrderLineValidator : AbstractValidator<SalesOrderLineDto>
{
    public SalesOrderLineValidator()
    {
        // 1. Identification (Changed ProductCode to ItemCode)
        RuleFor(x => x.ItemCode)
            .NotEmpty().WithMessage("Item code is required for every line.")
            .Matches(@"^[A-Z0-9-]+$").WithMessage("Invalid Item Code format.");

        // 2. Quantity Logic (Changed Quantity to QuantityOrdered)
        RuleFor(x => x.QuantityOrdered)
            .GreaterThan(0).WithMessage("Quantity ordered must be greater than zero.")
            .LessThan(10000).WithMessage("Quantity exceeds standard shipping limits.");

        // 3. Pricing
        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");
        // Removed the Must(>0) to allow for 0 price items if Discount/Promotion is applied

        // 4. Warehouse/Stock (Checking the Stock Issue flag)
        RuleFor(x => x.LineStatus)
            .NotEmpty().WithMessage("Line status is required.");
    }
}
