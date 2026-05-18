
namespace OperationalWorkspaceUI.State;

public class EventBus
{
    private readonly Dictionary<string,
        List<Action<object?>>> _subscriptions = new();

    // ======================================================
    // SUBSCRIBE
    // ======================================================

    public void Subscribe(
        string eventName,
        Action<object?> handler)
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
    // PUBLISH
    // ======================================================

    public void Publish(
        string eventName,
        object? data = null)
    {
        if (!_subscriptions.ContainsKey(eventName))
        {
            return;
        }

        foreach (var handler in _subscriptions[eventName])
        {
            handler.Invoke(data);
        }
    }
}