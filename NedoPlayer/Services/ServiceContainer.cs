using System;
using System.Collections.Generic;

namespace NedoPlayer.Services;

public class ServiceContainer : IServiceContainer
{
    private readonly Dictionary<Type, IService> _container;

    public ServiceContainer() => _container = new();
    
    public void Bind<T, TToBind>() where TToBind : IService, new() => _container[typeof(T)] = new TToBind();

    public T Get<T>() where T : IService => (T) _container[typeof(T)];
}
