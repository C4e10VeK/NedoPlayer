using System;
using System.Collections.ObjectModel;

namespace NedoPlayer.Models;

public class Playlist
{
    public ObservableCollection<MediaInfo>? MediaInfos { get; set; }
    public TimeSpan TotalDuration { get; set; }
}