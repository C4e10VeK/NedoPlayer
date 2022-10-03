namespace NedoPlayer.Services;

public class ServiceLocator
{
    private readonly ServiceContainer _container = new();
    public IServiceContainer Container => _container;

    public ServiceLocator()
    {
        _container.Bind<IFileService, FileService>();
        _container.Bind<IStateService, WindowStateService>();
        _container.Bind<IWindowService, WindowService>();
        _container.Bind<IConfigFileService, ConfigFileService>();
    }
}