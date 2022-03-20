namespace EventAggregator;

public class EventBase
{
    private List<ISubscription> _subscriptions = new List<ISubscription>();

    public List<ISubscription> Subscriptions
    {
        get => _subscriptions;
        set => _subscriptions = value ?? throw new ArgumentNullException(nameof(value));
    }
    
    
}