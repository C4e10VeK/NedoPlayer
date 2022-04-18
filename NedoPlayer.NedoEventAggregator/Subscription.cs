namespace NedoPlayer.NedoEventAggregator;

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

public class Subscription<T1, T2> : ISubscription
{
    private readonly Action<T1, T2>? _callback;
    
    public Subscription(Action<T1, T2>? callback, SubscriptionToken subscriptionToken)
    {
        _callback = callback;
        SubscriptionToken = subscriptionToken;
    }

    public void Invoke(T1 arg1, T2 arg2)
    {
        _callback?.Invoke(arg1, arg2);
    }

    public SubscriptionToken SubscriptionToken { get; set; }
}