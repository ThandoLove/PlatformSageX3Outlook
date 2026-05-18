namespace OperationalWorkspaceApplication.DTOs;

/// <summary>
/// Lightweight order model used in Email Intelligence UI.
/// </summary>
public class OpenOrderDto
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public DateTime? OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public string ClientName { get; set; } = "Unknown Client";

    // Optional: helps Email linking traceability
    public string? SourceEmailId { get; set; }
}