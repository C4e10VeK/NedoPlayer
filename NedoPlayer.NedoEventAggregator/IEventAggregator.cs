namespace NedoPlayer.NedoEventAggregator;

public interface IEventAggregator
{
    public T GetEvent<T>() where T : EventBase, new();
}