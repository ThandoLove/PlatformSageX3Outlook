using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject.Validators
{
    internal class LoginRequestValidator
    using FluentValidation;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspace.Shared.Validators;

    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username is too short.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
