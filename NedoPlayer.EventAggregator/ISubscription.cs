namespace EventAggregator;

public interface ISubscription
{
    SubscriptionToken SubscriptionToken { get; set; }
}