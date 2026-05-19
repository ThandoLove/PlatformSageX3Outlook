using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.State;

public class AppStateContainer
{
    // ======================================================
    // AUTH STATE
    // ======================================================

    public bool IsAuthenticated { get; private set; }
    public string? AccessToken { get; private set; }

    // ======================================================
    // EMAIL CONTEXT
    // ======================================================

    public EmailInsightDto? CurrentEmail { get; private set; }

    public string? CurrentEmailId =>
        CurrentEmail?.Id.ToString();

    public string? CurrentSubject =>
        CurrentEmail?.Subject;

    // ======================================================
    // CRM STATE
    // ======================================================

    public ClientDto? MatchedClient { get; private set; }

    public List<OrderDto> LinkedOrders { get; private set; } = new();
    public List<TaskDto> LinkedTasks { get; private set; } = new();

    // ======================================================
    // UI STATE
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
    // EMAIL (FIXED NULL SAFETY)
    // ======================================================

    public void SetCurrentEmail(EmailInsightDto email)
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

    public void SetMatchedClient(ClientDto? client)
    {
        MatchedClient = client;
        Notify();
    }

    public void SetLinkedOrders(List<OrderDto>? orders)
    {
        LinkedOrders = orders ?? new();
        Notify();
    }

    public void SetLinkedTasks(List<TaskDto>? tasks)
    {
        LinkedTasks = tasks ?? new();
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

    private void Notify() => OnChange?.Invoke();
}