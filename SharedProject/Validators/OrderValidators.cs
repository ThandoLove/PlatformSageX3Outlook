using FluentValidation;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceShared.Validators
{
    public class OrderValidator : AbstractValidator<OrderDto>
    {
        public OrderValidator()
        {
            // 1. Order Number check
            RuleFor(x => x.OrderNumber)
                .NotEmpty()
                .When(x => !string.IsNullOrEmpty(x.OrderNumber));

            // FIX: Changed CustomerId to ClientId to match your DTO
            RuleFor(x => x.ClientId)
                .NotEmpty()
                .WithMessage("Order must be linked to a Client.");

            // 2. Financial check
            RuleFor(x => x.TotalAmount)
                .GreaterThan(0)
                .WithMessage("Order total must be greater than zero.");

            // 3. Status check (Production Hardening)
            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required.");
        }
    }
}
