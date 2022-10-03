namespace NedoPlayer.Services;

public interface IStateService : IService
{
    public void SaveState(object? o);
    public void LoadState(object? o);
}