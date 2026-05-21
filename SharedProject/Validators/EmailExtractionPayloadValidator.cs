using FluentValidation;
using OperationalWorkspaceApplication.DTOs; // Binds directly to your real model namespace
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OperationalWorkspaceShared.Validators
{
    public class EmailInsightDtoValidator : AbstractValidator<EmailInsightDto>
    {
        public EmailInsightDtoValidator()
        {
            // =========================================================================
            // SENDER EMAIL ADDRESS VALIDATION MATRICES
            // =========================================================================
            RuleFor(x => x.From)
                .NotEmpty().WithMessage("Sender email address ('From') is required to query Sage X3 contexts.")
                .MaximumLength(256).WithMessage("Sender email address exceeds maximum structural length rules.")
                .Must(BeAValidEmailAddress).WithMessage("Sender email formatting does not match compliant specifications.");

            // =========================================================================
            // TRANSACTING MESSAGE TRACKING KEYS
            // =========================================================================
            RuleFor(x => x.MessageId)
                .NotEmpty().WithMessage("Outlook MessageId tracking token is mandatory for database interrogation loops.");

            // =========================================================================
            // SCRIPT INJECTION VECTOR SHIELD INTERCEPTOR
            // =========================================================================
            RuleFor(x => x)
                .Must(ExcludeCrossSiteScriptingPatterns)
                .WithMessage("Inbound transaction processing denied: Payload contains dangerous execution script patterns.");
        }

        private bool BeAValidEmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            // Verifies the structural string matches a standard corporate mailbox layout signature
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool ExcludeCrossSiteScriptingPatterns(EmailInsightDto payload)
        {
            if (payload == null) return false;

            // Scans the main text fields coming from the email context before they can be processed
            string textToScan = (payload.From + " " + payload.Subject + " " + payload.Message + " " + payload.MessageId).ToLowerInvariant();

            // Explicitly catches malicious text injection loops before data processing steps trigger
            string[] suspiciousScriptTokens =
            {
                "<script",
                "</script",
                "javascript:",
                "onerror=",
                "onload=",
                "<iframe",
                "xp_cmdshell",
                "exec("
            };

            return !suspiciousScriptTokens.Any(token => textToScan.Contains(token));
        }
    }
}
