using System;

namespace NedoPlayer.Services;

public interface IConfigFileService
{
    public string Read(string key, string? section = "config");
    public T Read<T>(string key, string? section = "config") where T : IConvertible;
    public void Write(string? key, string? value, string? section = "config");
    public bool KeyExists(string key, string? section = "config");
}