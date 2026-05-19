using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.ApplicationState;

public class AppStateContainer
{
    // ======================================================
    // AUTH
    // ======================================================

    public bool IsAuthenticated { get; private set; }

    public string? AccessToken { get; private set; }

    // ======================================================
    // EMAIL
    // ======================================================

    public EmailInsightDto? CurrentEmail { get; private set; }

    public string? CurrentEmailId =>
        CurrentEmail?.Id.ToString();

    public string? CurrentSubject =>
        CurrentEmail?.Subject;

    // ======================================================
    // CRM
    // ======================================================

    public BusinessPartnerSnapshotDto? MatchedClient { get; private set; }

    public List<OpenOrderDto> LinkedOrders { get; private set; }
        = new();

    public List<TaskDto> LinkedTasks { get; private set; }
        = new();

    // ======================================================
    // UI
    // ======================================================

    public bool IsBusy { get; private set; }

    // ======================================================
    // EVENTS
    // ======================================================

    public event Action? OnChange;

    // ======================================================
    // AUTH
    // ======================================================

    public void SetAuthentication(string token)
    {
        IsAuthenticated = true;

        AccessToken = token;

        Notify();
    }

    public void ClearAuthentication()
    {
        IsAuthenticated = false;

        AccessToken = null;

        Notify();
    }

    // ======================================================
    // EMAIL
    // ======================================================

    public void SetCurrentEmail(
        EmailInsightDto email)
    {
        CurrentEmail = email;

        Notify();
    }

    public void ClearCurrentEmail()
    {
        CurrentEmail = null;

        Notify();
    }

    // ======================================================
    // CRM
    // ======================================================

    public void SetMatchedClient(
        BusinessPartnerSnapshotDto? client)
    {
        MatchedClient = client;

        Notify();
    }

    public void SetLinkedOrders(
        List<OpenOrderDto>? orders)
    {
        LinkedOrders =
            orders ?? new List<OpenOrderDto>();

        Notify();
    }

    public void SetLinkedTasks(
        List<TaskDto>? tasks)
    {
        LinkedTasks =
            tasks ?? new List<TaskDto>();

        Notify();
    }

    // ======================================================
    // UI
    // ======================================================

    public void SetBusy(bool busy)
    {
        IsBusy = busy;

        Notify();
    }

    // ======================================================
    // FULL CONTEXT RESET
    // ======================================================

    public void ClearEmailContext()
    {
        CurrentEmail = null;

        MatchedClient = null;

        LinkedOrders.Clear();

        LinkedTasks.Clear();

        Notify();
    }

    // ======================================================
    // INTERNAL
    // ======================================================

    private void Notify()
    {
        OnChange?.Invoke();
    }
}