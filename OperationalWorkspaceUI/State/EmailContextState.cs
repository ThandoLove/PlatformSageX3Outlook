namespace OperationalWorkspaceUI.State;

public class EmailContextState
{
    // ======================================================
    // VIEW/UI STATE ONLY
    // ======================================================

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;

            NotifyStateChanged();
        }
    }

    // ======================================================
    // OPTIONAL UI HELPERS
    // ======================================================

    public string InitialSubject { get; set; }
        = string.Empty;

    public string InitialBody { get; set; }
        = string.Empty;

    public string InitialFrom { get; set; }
        = string.Empty;

    // ======================================================
    // EVENTS
    // ======================================================

    public event Action? OnChange;

    // ======================================================
    // INTERNAL
    // ======================================================

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}