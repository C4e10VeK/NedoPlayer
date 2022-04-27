﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
        private readonly DispatcherTimer _timerForHide;
        
        public MainWindow()
        {
            InitializeComponent();
            InitMediaPlayer();
            _timerForHide = new DispatcherTimer(DispatcherPriority.Input)
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _timerForHide.Tick += (_, _) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.None;
                    VideoPlayerControl.Visibility = Visibility.Collapsed;
                });
            };

            OpenMediaFromArgs();
        }

        private void OpenMediaFromArgs()
        {
            if (App.Args == null || !File.Exists(App.Args[0]) || DataContext is not MainViewModel dt) return;
            dt.OpenMediaFileInternal(App.Args[0]);
            dt.MediaControlController.OpenMediaFile(App.Args[0]);
        }

        private void InitMediaPlayer()
        {
            Unosquare.FFME.Library.FFmpegDirectory = @".\lib";

            if (DataContext is not MainViewModel dt) return;

            dt.MediaControlController.PlayRequested += async (_, _) => await VideoPlayer.Play();
            
            dt.MediaControlController.PauseRequested += async (_, _) => await VideoPlayer.Pause();

            dt.MediaControlController.CloseRequested += (_, _) => Close();

            dt.MediaControlController.MaximizeRequested += (_, fullscreen) =>
            {
                if (fullscreen)
                {
                    _timerForHide.IsEnabled = true;
                    VideoPlayerControl.SetValue(Grid.RowProperty, 1);
                    return;
                }
                
                _timerForHide.IsEnabled = false;
                VideoPlayerControl.SetValue(Grid.RowProperty, 2);
            };
            
            dt.MediaControlController.OpenMediaFileRequested += async (vm, path) => 
            {
                await VideoPlayer.Open(new Uri(path));
                if (vm is not MainViewModel viewModel) return;
                if (!viewModel.IsPaused)
                    await VideoPlayer.Play();
            };
        }

        private async void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            await VideoPlayer.Close();
        }

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DataContext is not MainViewModel {IsFullscreen: true})
            {
                Mouse.OverrideCursor = null;
                VideoPlayerControl.Visibility = Visibility.Visible;
                return;
            }
            
            _timerForHide.Stop();
            Mouse.OverrideCursor = null;
            VideoPlayerControl.Visibility = Visibility.Visible;
            _timerForHide.Start();
        }
    }
}