using System;

namespace NedoPlayer.Models
{
    public class MediaInfo
    {
        public string? Path { get; set; }
        public string? Title { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}