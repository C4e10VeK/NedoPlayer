using System;
using System.Collections.Generic;

namespace NedoPlayer.Services;

public class ServiceContainer : IServiceContainer
{
    private readonly Dictionary<Type, object> _container;

    public ServiceContainer() => _container = new();
    
    public void Bind<T, TToBind>() where TToBind : class, new() => _container[typeof(T)] = new TToBind();

    public T Get<T>() where T : class => (T) _container[typeof(T)];
}

public interface IServiceContainer
{
    public void Bind<T, TToBind>() where TToBind : class, new();
    public T Get<T>() where T : class;
}