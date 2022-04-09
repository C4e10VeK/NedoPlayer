namespace NedoPlayer.NedoEventAggregator;

public class SubscriptionToken : IDisposable, IEquatable<SubscriptionToken>
{
    private readonly Guid _token;
    private Action<SubscriptionToken>? _unsubscribe;

    public SubscriptionToken(Action<SubscriptionToken>? unsubscribe)
    {
        _unsubscribe = unsubscribe;
        _token = Guid.NewGuid();
    }

    public void Dispose()
    {
        _unsubscribe?.Invoke(this);
        _unsubscribe = null;
        
        GC.SuppressFinalize(this);
    }

    public bool Equals(SubscriptionToken? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _token.Equals(other._token) && Equals(_unsubscribe, other._unsubscribe);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((SubscriptionToken) obj);
    }

    public static bool operator ==(SubscriptionToken a, SubscriptionToken b) => a.Equals(b);

    public static bool operator !=(SubscriptionToken a, SubscriptionToken b) => !a.Equals(b);

    public override int GetHashCode()
    {
        return _token.GetHashCode();
    }
}