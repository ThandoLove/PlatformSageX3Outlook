namespace OperationalWorkspaceUI.State;

public sealed class EventBus
{
    // ======================================================
    // EVENT STORAGE
    // ======================================================

    private readonly Dictionary<
        string,
        List<Action<object?>>>
        _subscriptions = new();

    // ======================================================
    // THREAD SAFETY
    // ======================================================

    private readonly object _lock = new();

    // ======================================================
    // SUBSCRIBE
    // ======================================================

    public Action Subscribe(
        string eventName,
        Action<object?> handler)
    {
        lock (_lock)
        {
            if (!_subscriptions.ContainsKey(eventName))
            {
                _subscriptions[eventName] =
                    new List<Action<object?>>();
            }

            _subscriptions[eventName]
                .Add(handler);
        }

        // ======================================================
        // RETURN UNSUBSCRIBE ACTION
        // ======================================================

        return () => Unsubscribe(eventName, handler);
    }

    // ======================================================
    // UNSUBSCRIBE
    // ======================================================

    public void Unsubscribe(
        string eventName,
        Action<object?> handler)
    {
        lock (_lock)
        {
            if (!_subscriptions.ContainsKey(eventName))
            {
                return;
            }

            _subscriptions[eventName]
                .Remove(handler);

            // cleanup empty lists
            if (_subscriptions[eventName].Count == 0)
            {
                _subscriptions.Remove(eventName);
            }
        }
    }

    // ======================================================
    // PUBLISH
    // ======================================================

    public void Publish(
        string eventName,
        object? data = null)
    {
        List<Action<object?>> handlers;

        lock (_lock)
        {
            if (!_subscriptions.TryGetValue(
                    eventName,
                    out var existingHandlers))
            {
                return;
            }

            // clone prevents modification during iteration
            handlers = existingHandlers.ToList();
        }

        foreach (var handler in handlers)
        {
            try
            {
                handler.Invoke(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"EventBus handler error: {ex.Message}");
            }
        }
    }

    // ======================================================
    // CLEAR
    // ======================================================

    public void Clear()
    {
        lock (_lock)
        {
            _subscriptions.Clear();
        }
    }
}