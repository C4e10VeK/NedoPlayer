using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace NedoPlayer.Utils;

public class IniFile
{
    private string _path;
    private string _exe = Assembly.GetExecutingAssembly().GetName().Name;

    public IniFile(string? path = null)
    {
        _path = new FileInfo(path ?? _exe + ".ini").FullName;
    }

    public string Read(string key, string? section = null)
    {
        var retVal = new StringBuilder(255);
        GetPrivateProfileString(section ?? _exe, key, "", retVal, 255, _path);
        return retVal.ToString();
    }

    public T Read<T>(string key, string? section = null) where T : IConvertible
    {
        var res = Read(key, section);
        return (T) Convert.ChangeType(res, typeof(T), CultureInfo.InvariantCulture);
    }

    public void Write(string? key, string? value, string? section = null) =>
        WritePrivateProfileString(section ?? _exe, key, value, _path);

    public void DeleteKey(string? key, string? section = null) =>
        Write(key, null, section);

    public void DeleteSection(string? section = null) =>
        Write(null, null, section);

    public bool KeyExists(string key, string? section = null) => Read(key, section).Length > 0;

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string section, string? key, string? val, string filePath);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
        int size, string filePaths);
}