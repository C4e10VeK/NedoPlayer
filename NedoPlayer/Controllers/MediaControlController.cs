using System;
using NedoPlayer.ViewModels;

namespace NedoPlayer.Controllers;

public class MediaControlController
{
    public event EventHandler? CloseRequested;
    public event EventHandler? PlayPauseRequested;
    public event EventHandler<bool>? MaximizeRequested;
    public event EventHandler<string>? NextRequested;
    public event EventHandler<string>? PrevRequested;

    private readonly MainViewModel _mainViewModel;

    public MediaControlController(MainViewModel viewModel) => _mainViewModel = viewModel;

    public void PlayPause(object? s)
    {
        PlayPauseRequested?.Invoke(_mainViewModel, EventArgs.Empty);
        _mainViewModel.TaskBarIcon = _mainViewModel switch
        {
            {IsPaused: true} => "./Resources/img/pause.png",
            {IsPaused: false} => "./Resources/img/play.png",
            _ => _mainViewModel.TaskBarIcon
        };
    }

    public void Maximize(bool isFullscreen) => MaximizeRequested?.Invoke(_mainViewModel, isFullscreen);

    public void Next(object? s) => 
        NextRequested?.Invoke(_mainViewModel, string.Empty);

    public void Close(object? s) => 
        CloseRequested?.Invoke(_mainViewModel, EventArgs.Empty);
}