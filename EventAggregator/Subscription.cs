namespace EventAggregator;

public class Subscription : ISubscription
{
    private readonly Action? _callback;

    public Subscription(Action? callback, SubscriptionToken subscriptionToken)
    {
        _callback = callback;
        SubscriptionToken = subscriptionToken;
    }
    
    public void Invoke()
    {
        _callback?.Invoke();
    }

    public SubscriptionToken SubscriptionToken { get; set; }
}

public class Subscription<T> : ISubscription
{
    private readonly Action<T>? _callback;
    
    public Subscription(Action<T>? callback, SubscriptionToken subscriptionToken)
    {
        _callback = callback;
        SubscriptionToken = subscriptionToken;
    }

    public void Invoke(T arg)
    {
        _callback?.Invoke(arg);
    }

    public SubscriptionToken SubscriptionToken { get; set; }
}