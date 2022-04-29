using System;
using System.Collections.ObjectModel;

namespace NedoPlayer.Models;

public class Playlist : ModelBase
{
    private ObservableCollection<MediaInfo> _mediaInfos;

    public ObservableCollection<MediaInfo> MediaInfos
    {
        get => _mediaInfos;
        set
        {
            _mediaInfos = value;
            OnPropertyChanged(nameof(MediaInfos));
        }
    }

    private TimeSpan _totalDuration;

    public TimeSpan TotalDuration
    {
        get => _totalDuration;
        set
        {
            _totalDuration = value;
            OnPropertyChanged(nameof(TotalDuration));
        }
    }

    public Playlist()
    {
        _mediaInfos = new ObservableCollection<MediaInfo>();
        _totalDuration = TimeSpan.Zero;
    }

    public MediaInfo this[int index]
    {
        get => MediaInfos[index];
        set => MediaInfos[index] = value;
    }
}