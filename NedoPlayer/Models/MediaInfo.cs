using System;

namespace NedoPlayer.Models;

public class MediaInfo : ModelBase, IComparable<MediaInfo>, IEquatable<MediaInfo>
{

    public int GroupId { get; set; }
    public string Path { get; set; }
    public string Title { get; set; }
    public TimeSpan? Duration { get; set; }
    public int Repeat { get; set; }
    public bool IsPlaying { get; set; }
        
    public MediaInfo(int groupId, string path = "", string title = "", TimeSpan? duration = null)
    {
        GroupId = groupId;
        Path = path;
        Title = title;
        Duration = duration;
        Repeat = 0;
        IsPlaying = false;
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