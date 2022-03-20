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

    private MainViewModel? _mainViewModel;

    public MediaControlService(MainViewModel viewModel) => _mainViewModel = viewModel;

    private void PlayPause(object? s)
    {
        PlayPauseRequested?.Invoke(this, EventArgs.Empty);
        switch (_mainViewModel)
        {
            case {Paused: false}:
                _mainViewModel.PlayPauseKind = PackIconModernKind.ControlPlay;
                _mainViewModel.TaskBarIcon = "./Resources/play.png";
                break;
            case {Paused: true}:
                _mainViewModel.PlayPauseKind = PackIconModernKind.ControlPause;
                _mainViewModel.TaskBarIcon = "./Resources/pause.png";
                break;
        }
    }

    private void Next(object? s) => 
        NextRequested?.Invoke(this, string.Empty);

    private void Close(object? s) => 
        CloseRequested?.Invoke(this, EventArgs.Empty);
}