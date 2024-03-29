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

namespace NedoPlayer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly DispatcherTimer _timerForHide;
    private readonly DispatcherTimer _timerForPos;
        
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

        _timerForPos = new DispatcherTimer();
        _timerForPos.Interval = TimeSpan.FromMilliseconds(200);
        _timerForPos.Tick += TimerForPosOnTick;

        OpenMediaFromArgs();
    }

    private void TimerForPosOnTick(object sender, EventArgs e)
    {
        if (DataContext is not MainViewModel {MediaControlModel.TotalDuration.TotalSeconds: > 0} vm) return;
        if (!VideoPlayer.NaturalDuration.HasTimeSpan || !(VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds > 0)) return;
        
        vm.MediaControlModel.Position = VideoPlayer.Position;
    }

    private void OpenMediaFromArgs()
    {
        if (App.Args == null || !File.Exists(App.Args[0]) || DataContext is not MainViewModel dt) return;
        if (App.Args[0].ToLower().EndsWith(".nypl"))
        {
            dt.OpenPlaylistFile(App.Args[0]);
            return;
        }
        dt.OpenMediaFileInternal(App.Args[0]);
        dt.NextMediaCommand.Execute(null);
        dt.MediaControlModel.IsPaused = false;
    }

    private void InitMediaPlayer()
    {
        VideoPlayer.MediaOpened += (_, _) =>
        {
            _timerForPos.Start();
        };

        if (DataContext is not MainViewModel dt) return;

        dt.MediaControlController.PlayRequested += (_, _) => VideoPlayer.Play();
            
        dt.MediaControlController.PauseRequested += (_, _) => VideoPlayer.Pause();

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

        dt.MediaControlController.OpenMediaFileRequested += (vm, path) =>
        {
            if (VideoPlayer.Source is not null && !string.IsNullOrWhiteSpace(VideoPlayer.Source.ToString()))
                VideoPlayer.Close();
            VideoPlayer.Source = new Uri(path);
            VideoPlayer.Play();
            if (vm is not MainViewModel viewModel) return;
            if (viewModel.MediaControlModel.IsPaused)
                VideoPlayer.Pause();
        };

        dt.MediaControlController.CloseMediaRequested += (_, _) => VideoPlayer.Close();

        dt.MediaControlController.SeekRequested += (vm, position) =>
        {
            if (vm is not MainViewModel viewModel) return;
            if (viewModel.MediaControlModel.TotalDuration.TotalSeconds > 0) VideoPlayer.Position = position;
        };
    }

    private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (DataContext is not MainViewModel {MediaControlModel.IsFullscreen: true})
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