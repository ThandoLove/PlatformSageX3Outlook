
    using FluentValidation;
    using OperationalWorkspaceApplication.DTOs;

    namespace OperationalWorkspaceShared.Validators;

    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("You must select a client.");

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("Order date is required.");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than zero.");

            // OrderNumber might be empty if Sage X3 generates it automatically
        }
    }


