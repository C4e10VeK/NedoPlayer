using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using NedoPlayer.Models;
using NedoPlayer.Resources;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Application = System.Windows.Application;

namespace NedoPlayer.Services;

internal class FileService : IFileService
{
    public string OpenFileDialog(string path) => OpenFileDialog(path, Resource.MediaFileFilter);

    public string OpenFolderDialog(string path)
    {
        using var openFolderDialog = new CommonOpenFileDialog();
        openFolderDialog.InitialDirectory = path;
        openFolderDialog.IsFolderPicker = true;

        return openFolderDialog.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok ? openFolderDialog.FileName : string.Empty;
    }

    public void SavePlaylist(Playlist playlist)
    {
        using var saveFileDialog = new SaveFileDialog();
        saveFileDialog.InitialDirectory = @"C:\";
        saveFileDialog.Filter = @"nypl files (*.nypl)|*.nypl|All files|*.*";
        saveFileDialog.FilterIndex = 1;
        saveFileDialog.RestoreDirectory = true;
        
        if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
        var serialization = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var res = serialization.Serialize(playlist);
        Debug.Write(res);
        
        File.WriteAllText(saveFileDialog.FileName, res);
    }

    public Playlist OpenPlaylist(string path)
    {
        var fileName = OpenFileDialog(path, @"nypl files (*.nypl)|*.nypl");
        if (!File.Exists(fileName)) return new();
        
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var fileText = File.ReadAllText(fileName);
        var res = deserializer.Deserialize<Playlist>(fileText);

        return res;
    }

    private string OpenFileDialog(string path, string filter)
    {
        using var openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = path;
        openFileDialog.Filter = filter;
        openFileDialog.Multiselect = false;
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = true;

        return openFileDialog.ShowDialog() == DialogResult.OK ? openFileDialog.FileName : string.Empty;
    }
}