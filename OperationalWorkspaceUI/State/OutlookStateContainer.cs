
using System;
using OperationalWorkspaceApplication.DTOs;


namespace OperationalWorkspaceUI.State;

public interface IOutlookStateContainer
{
    string? CurrentMessageId { get; set; }
    string? CurrentSenderEmail { get; set; }
    BusinessPartnerSnapshotDto? ActivePartnerSnapshot { get; set; }
    event Action? OnStateChanged;
    void UpdateState(string messageId, string senderEmail, BusinessPartnerSnapshotDto? snapshot);
    void Reset();
}

public sealed class OutlookStateContainer : IOutlookStateContainer
{
    public string? CurrentMessageId { get; set; }
    public string? CurrentSenderEmail { get; set; }
    public BusinessPartnerSnapshotDto? ActivePartnerSnapshot { get; set; }

    public event Action? OnStateChanged;

    public void UpdateState(string messageId, string senderEmail, BusinessPartnerSnapshotDto? snapshot)
    {
        CurrentMessageId = messageId;
        CurrentSenderEmail = senderEmail;
        ActivePartnerSnapshot = snapshot;
        NotifyStateChanged();
    }

    public void Reset()
    {
        CurrentMessageId = null;
        CurrentSenderEmail = null;
        ActivePartnerSnapshot = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}
