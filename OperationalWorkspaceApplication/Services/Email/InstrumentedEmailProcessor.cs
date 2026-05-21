using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceApplication.Services.Email
{
    public class InstrumentedEmailProcessor
    {
        private static readonly ActivitySource ActivitySource =
            new ActivitySource("OperationalWorkspace.CoreEngine");

        private readonly ILogger<InstrumentedEmailProcessor> _logger;
        private readonly IEmailService _emailService;

        public InstrumentedEmailProcessor(
            ILogger<InstrumentedEmailProcessor> logger,
            IEmailService emailService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        // =========================================================
        // ENTERPRISE EMAIL CONTEXT PROCESSING
        // =========================================================
        public async Task<EmailContextDto?> ProcessAndTrackEmailContextAsync(
            string emailId,
            string correlationId)
        {
            using Activity? activity = ActivitySource.StartActivity("ProcessEmailTransaction");

            activity?.SetTag("email.id", emailId);
            activity?.SetTag("correlation.id", correlationId);

            using (_logger.BeginScope(
                new Dictionary<string, object>
                {
                    ["CorrelationId"] = correlationId,
                    ["EmailId"] = emailId
                }))
            {
                _logger.LogInformation(
                    "Beginning transactional tracing and insight calculation extraction for Email Message: {EmailId}",
                    emailId);

                // Validation safeguard parsing inbound trace tracking string tokens over to a crisp Guid context structure
                if (!Guid.TryParse(emailId, out Guid cleanEmailGuid) || cleanEmailGuid == Guid.Empty)
                {
                    _logger.LogWarning("Aborting diagnostic processing trace block: Inbound email token does not match an immutable Guid format definition layout.");
                    activity?.SetStatus(ActivityStatusCode.Error, "Invalid cryptographic transaction identifier token format mapping.");
                    return null;
                }

                try
                {
                    // =====================================================
                    // BUSINESS CONTEXT EXTRACTION (Type-Safe Handshake)
                    // =====================================================
                    var runtimeResult = await _emailService.GetEmailContextAsync(cleanEmailGuid);

                    if (runtimeResult != null)
                    {
                        _logger.LogInformation(
                            "Successfully mapped, auto-linked, and enriched email business context data arrays.");

                        activity?.SetStatus(ActivityStatusCode.Ok);
                        activity?.SetTag("business.partner", runtimeResult.Email?.BusinessPartnerName ?? "Unknown");
                        activity?.SetTag("linked.orders.count", runtimeResult.LinkedOrders.Count);
                        activity?.SetTag("linked.tasks.count", runtimeResult.LinkedTasks.Count);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Email extraction target processing query did not return a valid data structure context wrapper.");

                        activity?.SetStatus(ActivityStatusCode.Unset);
                    }

                    return runtimeResult;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(
                        ex,
                        "A fatal application process failure occurred while computing email transaction scopes.");

                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    throw;
                }
            }
        }
    }
}
