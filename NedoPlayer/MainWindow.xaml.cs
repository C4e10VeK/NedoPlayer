using System;
using NedoPlayer.ViewModels;

namespace NedoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitMediaPlayer();
        }

        private void InitMediaPlayer()
        {
            Unosquare.FFME.Library.FFmpegDirectory = @".\Resources";

            VideoPlayer.Open(new Uri(@"C:\Users\endar\Videos\Desktop\Desktop 2021.09.14 - 00.40.27.01.mp4"));

            if (DataContext is not MainViewModel dt) return;
            dt.PlayPauseRequested += async (_, _) =>
            {
                if (VideoPlayer.IsPlaying)
                {
                    await VideoPlayer.Pause();
                    dt.Paused = !VideoPlayer.IsPlaying;
                    return;
                }

                await VideoPlayer.Play();
                dt.Paused = VideoPlayer.IsPlaying;
            };

            dt.CloseRequested += (_, _) =>
            {
                Close();
            };

            VideoPlayer.MediaOpened += (_, _) =>
            {
                dt.TotalDuration = VideoPlayer.PlaybackEndTime.GetValueOrDefault(TimeSpan.Zero);
                dt.AppTitle = $"{VideoPlayer.Source.LocalPath} - {dt.AppTitle}";
            };
        }

        private async void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            await VideoPlayer.Close();
        }
    }
}