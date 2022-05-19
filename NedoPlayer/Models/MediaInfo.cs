using System;

namespace NedoPlayer.Models;

public class MediaInfo : ModelBase
{
    private int _groupId;

    public int GroupId
    {
        get => _groupId;
        set
        {
            _groupId = value;
            OnPropertyChanged(nameof(GroupId));
        }
    }

    private string _path;
    public string Path
    {
        get => _path;
        set
        {
            _path = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged(nameof(Path));
        }
    }

    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            _title = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged(nameof(Title));
        }
    }

    private TimeSpan? _duration;

    public TimeSpan? Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            OnPropertyChanged(nameof(Duration));
        }
    }

    private bool _isRepeat;

    public bool IsRepeat
    {
        get => _isRepeat;
        set
        {
            _isRepeat = value;
            OnPropertyChanged(nameof(IsRepeat));
        }
    }

    private bool _isPlaying;

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            _isPlaying = value;
            OnPropertyChanged(nameof(IsPlaying));
        }
    }

    public MediaInfo(int groupId, string path = "", string title = "", TimeSpan? duration = null)
    {
        _groupId = groupId;
        _path = path;
        _title = title;
        _duration = duration;
        _isRepeat = false;
        _isPlaying = false;
    }

    public override string ToString()
    {
        return $"Group Id: {GroupId}, Path: {Path}, Title: {Title}, Duration: {Duration?.ToString("hh\\:mm\\:ss")}";
    }
}