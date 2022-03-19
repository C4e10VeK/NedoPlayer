namespace EventAggregator;

public class PubSubEvent : EventBase
{
    public SubscriptionToken Subscribe(Action callback)
    {
        var newSubscription = new Subscription(callback, new SubscriptionToken(Unsubscribe));
        lock (Subscriptions)
        {
            Subscriptions.Add(newSubscription);
        }
        
        return newSubscription.SubscriptionToken;
    }

    public void Publish()
    {
        lock (Subscriptions)
        {
            foreach (var s in Subscriptions)
            {
                if (s is not Subscription sub) continue;
                
                sub.Invoke();
            }
        }
    }

    public void Unsubscribe(SubscriptionToken token)
    {
        lock (Subscriptions)
        {
            var sub = Subscriptions.FirstOrDefault(x => x.SubscriptionToken == token);

            if (sub != null)
                Subscriptions.Remove(sub);
        }
    }
}


public class PubSubEvent<T> : EventBase
{
    public SubscriptionToken Subscribe(Action<T> callback)
    {
        var newSubscription = new Subscription<T>(callback, new SubscriptionToken(Unsubscribe));

        lock (Subscriptions)
        {
            Subscriptions.Add(newSubscription);
        }
        
        return newSubscription.SubscriptionToken;
    }
    
    public void Publish(T arg)
    {
        lock (Subscriptions)
        {
            foreach (var s in Subscriptions)
            {
                if (s is not Subscription<T> sub) continue;
                
                sub.Invoke(arg);
            }
        }
    }

    public void Unsubscribe(SubscriptionToken token)
    {
        lock (Subscriptions)
        {
            var sub = Subscriptions.FirstOrDefault(x => x.SubscriptionToken == token);

            if (sub != null)
                Subscriptions.Remove(sub);
        }
    }
}