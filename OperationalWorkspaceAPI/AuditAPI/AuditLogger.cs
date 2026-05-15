using Microsoft.Extensions.Logging;

namespace OperationalWorkspaceAPI.AuditAPI;


public sealed class AuditLogger : IAuditLogger
{
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(ILogger<AuditLogger> logger)
    {
        _logger = logger;
    }

    public Task LogAsync(AuditEvent auditEvent)
    {
        _logger.LogInformation(
            """
            AUDIT:
            UserId: {UserId}
            Email: {Email}
            Company: {Company}
            Dataset: {Dataset}
            Action: {Action}
            Source: {Source}
            Method: {Method}
            Success: {Success}
            Error: {Error}
            Timestamp: {Timestamp}
            """,
            auditEvent.UserId,
            auditEvent.Email,
            auditEvent.Company,
            auditEvent.Dataset,
            auditEvent.Action,
            auditEvent.Source,
            auditEvent.HttpMethod,
            auditEvent.Success,
            auditEvent.ErrorMessage,
            auditEvent.TimestampUtc
        );

        return Task.CompletedTask;
    }
}