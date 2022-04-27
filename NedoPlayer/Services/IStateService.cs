namespace NedoPlayer.Services;

public interface IStateService
{
    public void SaveState(object? o);
    public void LoadState(object? o);
}