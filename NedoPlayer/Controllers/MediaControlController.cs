using System;
using NedoPlayer.ViewModels;

namespace NedoPlayer.Controllers;

public class MediaControlController
{
    public event EventHandler? CloseRequested;
    public event EventHandler? PlayPauseRequested;
    public event EventHandler<string>? OpenMediaFileRequested;
    public event EventHandler<bool>? MaximizeRequested;

    private readonly MainViewModel _mainViewModel;

    public MediaControlController(MainViewModel viewModel) => _mainViewModel = viewModel;

    public void PlayPause() => PlayPauseRequested?.Invoke(_mainViewModel, EventArgs.Empty);

    public void Maximize(bool isFullscreen) => MaximizeRequested?.Invoke(_mainViewModel, isFullscreen);

    public void Next(string path) => OpenMediaFile(path);

    public void Prev(string path) => OpenMediaFile(path);

    public void Close(object? s) => CloseRequested?.Invoke(_mainViewModel, EventArgs.Empty);

    public void OpenMediaFile(string filePath) => OpenMediaFileRequested?.Invoke(_mainViewModel, filePath);
}