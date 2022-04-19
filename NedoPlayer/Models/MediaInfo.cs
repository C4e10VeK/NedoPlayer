using System;

namespace NedoPlayer.Models
{
    public class MediaInfo
    {
        public int GroupId { get; set; }
        public string? Path { get; set; }
        public string? Title { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}