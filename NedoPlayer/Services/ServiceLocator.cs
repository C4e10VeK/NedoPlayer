﻿namespace NedoPlayer.Services;

public class ServiceLocator
{
    public static ServiceLocator Instance { get; } = new();

    public IServiceContainer Container { get; }

    private ServiceLocator()
    {
        Container = new ServiceContainer();
        Container.Bind<IFileService, FileService>();
        Container.Bind<IStateService, WindowStateService>();
        Container.Bind<IWindowService, WindowService>();
        Container.Bind<IConfigFileService, ConfigFileService>();
    }
}