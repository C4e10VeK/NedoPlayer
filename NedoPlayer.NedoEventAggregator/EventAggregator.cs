namespace NedoPlayer.NedoEventAggregator;

public class EventAggregator : IEventAggregator
{
    private static EventAggregator? _instance;
    public static IEventAggregator Instance => _instance ??= new EventAggregator();
    
    private readonly Dictionary<Type, EventBase> _events = new Dictionary<Type, EventBase>();

    public T GetEvent<T>() where T : EventBase, new()
    {
        lock (_events)
        {
            if (_events.TryGetValue(typeof(T), out var existingEvent)) return (T) existingEvent;
            
            var newEvent = new T();
            _events[typeof(T)] = newEvent;

            return newEvent;
        }
    }
}