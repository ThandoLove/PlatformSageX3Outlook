using FluentValidation;
using OperationalWorkspaceApplication.DTOs;
using System.Text.RegularExpressions;

namespace OperationalWorkspaceShared.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        // ==========================
        // USERNAME VALIDATION
        // ==========================
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username is too short.")
            .MaximumLength(50).WithMessage("Username is too long.")
            .Must(BeValidUsername).WithMessage("Username contains invalid characters.");

        // ==========================
        // PASSWORD VALIDATION
        // ==========================
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(100).WithMessage("Password is too long.")
            .Must(ContainUppercase).WithMessage("Password must contain at least one uppercase letter.")
            .Must(ContainLowercase).WithMessage("Password must contain at least one lowercase letter.")
            .Must(ContainDigit).WithMessage("Password must contain at least one number.")
            .Must(NotContainSpaces).WithMessage("Password cannot contain spaces.");

        // ==========================
        // CROSS-FIELD RULE (optional hardening)
        // ==========================
        RuleFor(x => x)
            .Must(NoInjectionPatterns)
            .WithMessage("Input contains invalid patterns.");
    }

    // ==========================
    // USERNAME RULES
    // ==========================
    private bool BeValidUsername(string username)
    {
        // Allows: letters, numbers, underscore, dot
        return Regex.IsMatch(username, @"^[a-zA-Z0-9_.]+$");
    }

    // ==========================
    // PASSWORD RULES
    // ==========================
    private bool ContainUppercase(string password)
        => password.Any(char.IsUpper);

    private bool ContainLowercase(string password)
        => password.Any(char.IsLower);

    private bool ContainDigit(string password)
        => password.Any(char.IsDigit);

    private bool NotContainSpaces(string password)
        => !password.Contains(" ");

    // ==========================
    // BASIC INJECTION PROTECTION
    // ==========================
    private bool NoInjectionPatterns(LoginRequestDto dto)
    {
        if (dto.Username == null || dto.Password == null)
            return false;

        string input = dto.Username + dto.Password;

        string[] bannedPatterns =
        {
            "--",
            "';",
            "/*",
            "*/",
            "xp_",
            "drop ",
            "select ",
            "insert ",
            "delete ",
            "update "
        };

        return !bannedPatterns.Any(p =>
            input.ToLower().Contains(p));
    }
}