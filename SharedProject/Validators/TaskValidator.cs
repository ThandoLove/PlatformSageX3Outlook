
    using FluentValidation;
    using OperationalWorkspaceApplication.DTOs;

    namespace OperationalWorkspaceShared.Validators;

    public class TaskValidator : AbstractValidator<TaskDto>
    {
        public TaskValidator()
        {
            RuleFor(x => x.Title).NotEmpty().Length(5, 100);
            RuleFor(x => x.DueDate).GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");
            RuleFor(x => x.AssignedTo).NotEmpty().WithMessage("Task must be assigned to an employee.");
        }
    }


