using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;


using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceShared.Validators;

    public class TicketValidator : AbstractValidator<TicketDto>
    {
        public TicketValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Ticket title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Please provide a description of the issue.")
                .MinimumLength(10).WithMessage("Description is too brief.");

        RuleFor(x => x.Priority)
 .NotEmpty().WithMessage("Priority is required.")
 .Must(p => int.TryParse(p, out var val) && val >= 1 && val <= 5)
 .WithMessage("Priority must be between 1 (Low) and 5 (Critical).");

    }
}


