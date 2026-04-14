
    using FluentValidation;
    using OperationalWorkspaceApplication.DTOs;

    namespace OperationalWorkspaceShared.Validators;

    public class SalesOrderValidator : AbstractValidator<SalesOrderDto>
    {
        public SalesOrderValidator()
        {
            RuleFor(x => x.BusinessPartnerCode)
                .NotEmpty().WithMessage("Business Partner Code is required.");

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0);

            // This is the production standard: Validate all lines automatically
            RuleForEach(x => x.Lines).SetValidator(new SalesOrderLineValidator());
        }
    }


