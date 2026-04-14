using FluentValidation;
using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;



namespace OperationalWorkspaceShared.Validators;

    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Sage Customer ID is required.")
                .Matches(@"^[A-Z0-9]+$").WithMessage("ID must be uppercase alphanumeric (Sage Standard).")
                .Length(3, 10).WithMessage("ID must be between 3 and 10 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter a valid email address.");

            RuleFor(x => x.CreditLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Credit limit cannot be negative.");
        }
    }

    

