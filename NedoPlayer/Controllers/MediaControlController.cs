using System;
using NedoPlayer.ViewModels;

namespace NedoPlayer.Controllers;

public class MediaControlController
{
    public event EventHandler? CloseRequested;
    public event EventHandler? PlayRequested;
    public event EventHandler? PauseRequested;
    public event EventHandler<string>? OpenMediaFileRequested;
    public event EventHandler<bool>? MaximizeRequested;

    private readonly MainViewModel _mainViewModel;

    public MediaControlController(MainViewModel viewModel) => _mainViewModel = viewModel;
    
    /// <summary>
    /// Play media in MediaElement.
    /// </summary>
    public void Play() => PlayRequested?.Invoke(_mainViewModel, EventArgs.Empty);
    
    /// <summary>
    /// Pause media in MediaElement
    /// </summary>
    public void Pause() => PauseRequested?.Invoke(_mainViewModel, EventArgs.Empty);

    /// <summary>
    /// Call maximize event handler for set fullscreen state in main window elements 
    /// </summary>
    /// <param name="isFullscreen">fullscreen state as boolean</param>
    public void Maximize(bool isFullscreen) => MaximizeRequested?.Invoke(_mainViewModel, isFullscreen);

    /// <summary>
    /// Close the main window
    /// </summary>
    public void Close() => CloseRequested?.Invoke(_mainViewModel, EventArgs.Empty);

    /// <summary>
    /// Open media file from path
    /// </summary>
    /// <param name="filePath">File path</param>
    public void OpenMediaFile(string filePath) => OpenMediaFileRequested?.Invoke(_mainViewModel, filePath);
}