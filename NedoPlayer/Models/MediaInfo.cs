using System;

namespace NedoPlayer.Models;

public class MediaInfo : ModelBase, IComparable<MediaInfo>, IEquatable<MediaInfo>
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

    private int _repeat;

    public int Repeat
    {
        get => _repeat;
        set
        {
            _repeat = value;
            OnPropertyChanged(nameof(Repeat));
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
        _repeat = 0;
        _isPlaying = false;
    }

    public int CompareTo(MediaInfo? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        return ReferenceEquals(null, other) ? 1 : GroupId.CompareTo(other.GroupId);
    }

    public bool Equals(MediaInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return GroupId == other.GroupId;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MediaInfo)obj);
    }

    public override string ToString()
    {
        return $"Group Id: {GroupId}, Path: {Path}, Title: {Title}, Duration: {Duration?.ToString("hh\\:mm\\:ss")}";
    }
}