using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    private struct InternalMediaInfo
    {
        public int GroupId;
        public string Path;
        public string Title;
        public TimeSpan? Duration;
    }

    public string OpenFileDialog(string path) => OpenFileDialog(path, Resource.MediaFileFilter);
    
    public string OpenFileDialog(string path, string filter)
    {
        using var openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = path;
        openFileDialog.Filter = filter;
        openFileDialog.Multiselect = false;
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = true;

        return openFileDialog.ShowDialog() == DialogResult.OK ? openFileDialog.FileName : string.Empty;
    }

    public string OpenFolderDialog(string path)
    {
        using var openFolderDialog = new CommonOpenFileDialog();
        openFolderDialog.InitialDirectory = path;
        openFolderDialog.IsFolderPicker = true;

        return openFolderDialog.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok ? openFolderDialog.FileName : string.Empty;
    }

    public void SavePlaylist(Playlist playlist, string startPath)
    {
        using var saveFileDialog = new SaveFileDialog();
        saveFileDialog.InitialDirectory = startPath;
        saveFileDialog.Filter = @"nypl files (*.nypl)|*.nypl|All files|*.*";
        saveFileDialog.FilterIndex = 1;
        saveFileDialog.RestoreDirectory = true;

        if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

        List<InternalMediaInfo> internalMediaInfos = playlist.MediaInfos.Select(mediaInfo => new InternalMediaInfo
        {
            GroupId = mediaInfo.GroupId, Path = mediaInfo.Path, Title = mediaInfo.Title, Duration = mediaInfo.Duration
        }).ToList();

        var serialization = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var res = serialization.Serialize(internalMediaInfos);
        Debug.Write(res);
        
        File.WriteAllText(saveFileDialog.FileName, res);
    }

    public Playlist OpenPlaylist(string fileName)
    {
        if (!File.Exists(fileName)) return new Playlist();
        
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var fileText = File.ReadAllText(fileName);
        var internalMediaInfos = deserializer.Deserialize<List<InternalMediaInfo>>(fileText)
            .Select(x => new MediaInfo(x.GroupId, x.Path, x.Title, x.Duration));

        var res = new Playlist
        {
            MediaInfos = new ObservableCollection<MediaInfo>(internalMediaInfos)
        };
        
        return res;
    }
}