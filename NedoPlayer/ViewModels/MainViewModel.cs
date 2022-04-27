using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using NedoPlayer.Controllers;
using NedoPlayer.Models;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Services;
using NedoPlayer.Utils;
using Unosquare.FFME.Common;
using MediaInfo = NedoPlayer.Models.MediaInfo;

namespace NedoPlayer.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    public MediaControlController MediaControlController { get; }
    private readonly IOService _fileDialogService;
    private readonly IStateService _windowStateService;
    private int _playedMediaIndex;

    private string _trackTitle;
    public string TrackTitle
    {
        get => _trackTitle;
        set
        {
            _trackTitle = value;
            OnPropertyChanged(nameof(TrackTitle));
        }
    }

    private TimeSpan _playerTimeToEnd;
    public TimeSpan PlayerTimeToEnd
    {
        get => _playerTimeToEnd;
        set
        {
            _playerTimeToEnd = value;
            NotifySliderDataChanged();
        }
    }

    public double PlayerTimeToEndSeconds
    {
        get => PlayerTimeToEnd.TotalSeconds;
        set => PlayerTimeToEnd = TimeSpan.FromSeconds(value);
    }

    private TimeSpan _totalDuration;
    public TimeSpan TotalDuration
    {
        get => _totalDuration;
        set
        {
            _totalDuration = value;
            OnPropertyChanged(nameof(TotalDuration));
        }
    }

    private bool _isPaused;
    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            _isPaused = value;
            OnPropertyChanged(nameof(IsPaused));
        }
    }

    private bool _isFullscreen;
    public bool IsFullscreen
    {
        get => _isFullscreen;
        set
        {
            _isFullscreen = value;
            OnPropertyChanged(nameof(IsFullscreen));
        }
    }

    private double _volume;
    public double Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            VolumeToFfmpeg = value / 100;
            OnPropertyChanged(nameof(Volume));
        }
    }

    private double _volumeToFfmpeg;
    public double VolumeToFfmpeg
    {
        get => _volumeToFfmpeg;
        set
        {
            _volumeToFfmpeg = value;
            OnPropertyChanged(nameof(VolumeToFfmpeg));
        }
    }

    private bool _isMuted;
    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            _isMuted = value;
            OnPropertyChanged(nameof(IsMuted));
        }
    }

    private bool _isPlayListOpened;
    public bool IsPlayListOpened
    {
        get => _isPlayListOpened;
        set
        {
            _isPlayListOpened = value;
            OnPropertyChanged(nameof(IsPlayListOpened));
        }
    }

    private Playlist _playlist;
    public Playlist Playlist
    {
        get => _playlist;
        set
        {
            _playlist = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged(nameof(Playlist));
        }
    }

    public ICommand CloseCommand { get; }
    public ICommand PlayPauseCommand { get; }
    public ICommand MaximizeCommand { get; }
    public ICommand MuteCommand { get; }
    public ICommand MediaOpenedCommand { get; }
    public ICommand OpenPlaylistCommand { get; }
    public ICommand SeekStartCommand { get; }
    public ICommand SeekEndCommand { get; }
    public ICommand MediaEndedCommand { get; }

    public ICommand OpenFileCommand { get; }
    public ICommand NextMediaCommand { get; }
    public ICommand PreviousMediaCommand { get; }

    public MainViewModel(IEventAggregator aggregator, IOService fileService, IStateService windowStateService) : base(aggregator)
    {
        MediaControlController = new MediaControlController(this);
        _fileDialogService = fileService;
        _windowStateService = windowStateService;
        _trackTitle = "";
        _isPaused = true;
        _playerTimeToEnd = TimeSpan.FromSeconds(0);
        _totalDuration = TimeSpan.Zero;
        _volume = 100;
        _volumeToFfmpeg = _volume / 100;
        _isMuted = false;
        _isPlayListOpened = false;
        _playedMediaIndex = 0;
        _playlist = new Playlist();
        _playedMediaIndex = -1;

        CloseCommand = new RelayCommand(_ => MediaControlController.Close());
        PlayPauseCommand = new RelayCommand(_ =>
        {
            if (IsPaused)
            {
                MediaControlController.Play();
                IsPaused = false;
                return;
            }
            MediaControlController.Pause();
            IsPaused = true;
        });
        MaximizeCommand = new RelayCommand(Maximize);
        MuteCommand = new RelayCommand(_ => IsMuted = !IsMuted);
        MediaOpenedCommand = new RelayCommand(MediaOpened);
        OpenPlaylistCommand = new RelayCommand(_ => IsPlayListOpened = !IsPlayListOpened);
        SeekStartCommand = new RelayCommand(_ =>
        {
            MediaControlController.Pause();
        });
        SeekEndCommand = new RelayCommand(_ => MediaControlController.Play());
        MediaEndedCommand = new RelayCommand(MediaEnded);
            
        OpenFileCommand = new RelayCommand(OpenMediaFile);
        NextMediaCommand = new RelayCommand(NextMediaFile, _ => _playedMediaIndex < Playlist.MediaInfos.Count - 1);
        PreviousMediaCommand = new RelayCommand(PreviousMediaFile, _ => _playedMediaIndex > 0);
    }

    /// <summary>
    /// Maximize main window
    /// </summary>
    /// <param name="s"></param>
    private void Maximize(object? s)
    {
        if (Application.Current.MainWindow is not MetroWindow wnd) return;

        if (!_isFullscreen)
        {
            _windowStateService.SaveState(wnd);
                
            wnd.WindowState = WindowState.Maximized;
            wnd.WindowStyle = WindowStyle.None;
            wnd.UseNoneWindowStyle = true;
            wnd.IgnoreTaskbarOnMaximize = true;

            IsFullscreen = true;
            MediaControlController.Maximize(IsFullscreen);
            return;
        }
            
        _windowStateService.LoadState(wnd);
        wnd.IgnoreTaskbarOnMaximize = false;
        wnd.UseNoneWindowStyle = false;
        wnd.ShowTitleBar = true;

        IsFullscreen = false;
        MediaControlController.Maximize(IsFullscreen);
    }

    /// <summary>
    /// Set next media file in container
    /// </summary>
    /// <param name="o"></param>
    private void NextMediaFile(object? o)
    {
        Playlist.MediaInfos[_playedMediaIndex].IsPlaying = false;
        ++_playedMediaIndex;

        Playlist.MediaInfos[_playedMediaIndex].IsPlaying = true;
        MediaInfo info = Playlist.MediaInfos[_playedMediaIndex];
        MediaControlController.OpenMediaFile(info.Path + info.Title);
    }

    /// <summary>
    /// set previous media file in container
    /// </summary>
    /// <param name="o"></param>
    private void PreviousMediaFile(object? o)
    {
        Playlist.MediaInfos[_playedMediaIndex].IsPlaying = false;
        --_playedMediaIndex;

        Playlist.MediaInfos[_playedMediaIndex].IsPlaying = true;
        MediaInfo info = Playlist.MediaInfos[_playedMediaIndex];
        MediaControlController.OpenMediaFile(info.Path + info.Title);
    }

    /// <summary>
    /// Call with media opened event. using for set value for media element control
    /// </summary>
    /// <param name="o">Args from media opened event</param>
    private void MediaOpened(object? o)
    {
        if (o is not MediaOpenedEventArgs args)
            return;
            
        TotalDuration = args.Info.Duration;
        PlayerTimeToEnd = args.Info.StartTime;
        TrackTitle = $"{args.Info.MediaSource}";
        if (!IsPaused) 
            MediaControlController.Play();
    }
    
    /// <summary>
    /// Call with media opened event. Using for set next media from playlist if exist
    /// </summary>
    /// <param name="obj"></param>
    private void MediaEnded(object? obj)
    {
        if (!Playlist.MediaInfos.Any() || _playedMediaIndex == Playlist.MediaInfos.Count - 1)
            return;
            
        Playlist.MediaInfos[_playedMediaIndex].IsPlaying = false;
        ++_playedMediaIndex;
        Playlist.MediaInfos[_playedMediaIndex].IsPlaying = true;
        MediaInfo info = Playlist.MediaInfos[_playedMediaIndex];
        MediaControlController.OpenMediaFile(info.Path + info.Title);
    }

    /// <summary>
    /// Method for open media file. Using in button command
    /// </summary>
    /// <param name="o"></param>
    private void OpenMediaFile(object? o)
    {
        string filePath = _fileDialogService.OpenFileDialog(@"C:\");
        if (string.IsNullOrWhiteSpace(filePath)) return;
        
        OpenMediaFileInternal(filePath);
        RunMediaIfFirst();
    }

    /// <summary>
    /// Open internal media file
    /// </summary>
    /// <param name="filePath"></param>
    internal void OpenMediaFileInternal(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return;
        string title = Path.GetFileName(filePath);

        // Sorting medias by group
        var tempMediaList = Playlist.MediaInfos.OrderBy(x => x).ToList();
        Debug.Assert(Playlist != null, "Playlist != null");
        // get group medias by path
        List<MediaInfo> groupList = Playlist!.MediaInfos.Where(x => x.Path == filePath.Replace(title, "")).ToList();
        
        // set group id for new media
        int groupId = tempMediaList.Count == 0 ?
            0 : !groupList.Any() ? tempMediaList.Last().GroupId + 1 : groupList.First().GroupId;

        // Get media duration 
        using ShellObject shell = ShellObject.FromParsingName(filePath);
        ShellProperty<ulong?> prop = shell.Properties.System.Media.Duration;
        ulong duration = prop.Value ?? 0;
        
        MediaInfo mediaAdd = new MediaInfo(groupId, filePath.Replace(title, ""), title, TimeSpan.FromTicks((long)duration));
            
        int lastIndexGroup = !groupList.Any() ? Playlist.MediaInfos.Count : Playlist.MediaInfos.IndexOf(groupList.Last()) + 1;

        if (Playlist.MediaInfos.Any(x => x.Path == mediaAdd.Path && x.Title == mediaAdd.Title))
            return;

        Playlist.MediaInfos.Insert(lastIndexGroup, mediaAdd);
            
        CountPlaylistDuration();
    }

    /// <summary>
    /// Run media from playlist if it's media is only one
    /// </summary>
    private void RunMediaIfFirst()
    {
        if (Playlist.MediaInfos.Count > 1 || _playedMediaIndex > -1)
            return;
            
        string path = Playlist.MediaInfos.First().Path + Playlist.MediaInfos.First().Title;
        MediaControlController.OpenMediaFile(path);
        Playlist.MediaInfos.First().IsPlaying = true;
        MediaControlController.Play();
        IsPaused = false;
        ++_playedMediaIndex;
    }
    
    /// <summary>
    /// Count total duration of playlist
    /// </summary>
    private void CountPlaylistDuration()
    {
        foreach (var mediaInfo in Playlist.MediaInfos)
            Playlist.TotalDuration += mediaInfo.Duration.GetValueOrDefault();
    }

    private void NotifySliderDataChanged()
    {
        OnPropertyChanged(nameof(PlayerTimeToEnd));
        OnPropertyChanged(nameof(PlayerTimeToEndSeconds));
    }
}