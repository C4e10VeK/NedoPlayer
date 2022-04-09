using System;
using MahApps.Metro.IconPacks;
using NedoPlayer.ViewModels;

namespace NedoPlayer.Services;

public class MediaControlService
{
    public event EventHandler? CloseRequested;
    public event EventHandler? PlayPauseRequested;
    public event EventHandler<string>? NextRequested;
    public event EventHandler<string>? PrevRequested;
    public event EventHandler<double>? VolumeRequested;

    private readonly MainViewModel _mainViewModel;

    public MediaControlService(MainViewModel viewModel) => _mainViewModel = viewModel;

    public void PlayPause(object? s)
    {
        PlayPauseRequested?.Invoke(this, EventArgs.Empty);
        switch (_mainViewModel)
        {
            case {Paused: false}:
                _mainViewModel.PlayPauseKind = PackIconModernKind.ControlPlay;
                _mainViewModel.TaskBarIcon = "./Resources/img/play.png";
                break;
            case {Paused: true}:
                _mainViewModel.PlayPauseKind = PackIconModernKind.ControlPause;
                _mainViewModel.TaskBarIcon = "./Resources/img/pause.png";
                break;
        }
    }

    public void Next(object? s) => 
        NextRequested?.Invoke(this, string.Empty);

    public void Close(object? s) => 
        CloseRequested?.Invoke(this, EventArgs.Empty);

    public void ChangeVolume(object? s, double volume) => VolumeRequested?.Invoke(s, volume);
}