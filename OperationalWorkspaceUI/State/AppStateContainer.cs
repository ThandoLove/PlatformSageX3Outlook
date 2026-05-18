
namespace OperationalWorkspaceUI.State;

public class AppStateContainer
{
    // ======================================================
    // AUTH
    // ======================================================

    public bool IsAuthenticated { get; private set; }

    public string? AccessToken { get; private set; }

    // ======================================================
    // EMAIL CONTEXT
    // ======================================================

    public string? CurrentEmailId { get; private set; }

    public string? CurrentSubject { get; private set; }

    // ======================================================
    // UI STATE
    // ======================================================

    public bool IsBusy { get; private set; }

    // ======================================================
    // EVENTS
    // ======================================================

    public event Action? OnChange;

    // ======================================================
    // AUTH METHODS
    // ======================================================

    public void SetAuthentication(string token)
    {
        IsAuthenticated = true;
        AccessToken = token;

        NotifyStateChanged();
    }

    public void ClearAuthentication()
    {
        IsAuthenticated = false;
        AccessToken = null;

        NotifyStateChanged();
    }

    // ======================================================
    // EMAIL METHODS
    // ======================================================

    public void SetCurrentEmail(
        string emailId,
        string subject)
    {
        CurrentEmailId = emailId;
        CurrentSubject = subject;

        NotifyStateChanged();
    }

    // ======================================================
    // UI METHODS
    // ======================================================

    public void SetBusy(bool busy)
    {
        IsBusy = busy;

        NotifyStateChanged();
    }

    // ======================================================
    // INTERNAL
    // ======================================================

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}