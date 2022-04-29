using System;
using NedoPlayer.Utils;

namespace NedoPlayer.Services;

public class ConfigFileService : IConfigFileService
{
    private IniFile _iniFile = new("./config.ini");

    public string Read(string key, string? section = "config") => _iniFile.Read(key, section);
    public T Read<T>(string key, string? section = "config") where T : IConvertible => _iniFile.Read<T>(key, section);
    public void Write(string? key, string? value, string? section = "config") => _iniFile.Write(key, value, section);
    public bool KeyExists(string key, string? section = "config") => _iniFile.KeyExists(key, section);
}