using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using NedoPlayer.ViewModels;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

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

        private DispatcherTimer _foo;

        private void InitMediaPlayer()
        {
            Unosquare.FFME.Library.FFmpegDirectory = @".\Resources";

            VideoPlayer.Open(new Uri(@"C:\Users\endar\Downloads\INCENDIMUS [Death Knight Theme].mp4"));

            _foo = new DispatcherTimer(DispatcherPriority.Input)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _foo.Tick += (_, _) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.None;
                    VideoPlayerControl.Visibility = Visibility.Collapsed;
                });
            }; 

            if (DataContext is not MainViewModel dt) return;
            dt.ControlService.PlayPauseRequested += async (_, _) =>
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

            dt.ControlService.VolumeRequested += (sender, vol) =>
            {
                VideoPlayer.Volume = vol;
            }; 

            dt.ControlService.CloseRequested += (_, _) => Close();

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

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DataContext is not MainViewModel {FullscreenVisible: Visibility.Collapsed})
            {
                _foo.IsEnabled = false;
                Mouse.OverrideCursor = null;
                VideoPlayerControl.Visibility = Visibility.Visible;
                return;
            }
            
            _foo.IsEnabled = true;
            _foo.Stop();
            Mouse.OverrideCursor = null;
            VideoPlayerControl.Visibility = Visibility.Visible;
            _foo.Start();
        }
    }
}