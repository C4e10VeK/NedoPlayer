namespace NedoPlayer.Services;

public interface IServiceContainer
{
    public void Bind<T, TToBind>() where TToBind : IService, new();
    public T Get<T>() where T : IService;
}