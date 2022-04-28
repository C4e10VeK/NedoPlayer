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

    private TimeSpan _position;
    public TimeSpan Position
    {
        get => _position;
        set
        {
            _position = value;
            NotifySliderDataChanged();
        }
    }

    public double PositionSeconds
    {
        get => Position.TotalSeconds;
        set => Position = TimeSpan.FromSeconds(value);
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

    private int _selectedMediaIndex;
    public int SelectedMediaIndex
    {
        get => _selectedMediaIndex;
        set
        {
            _selectedMediaIndex = value;
            OnPropertyChanged(nameof(SelectedMediaIndex));
        }
    }

    private MediaInfo _currentMedia;

    public MediaInfo CurrentMedia
    {
        get => _currentMedia;
        set
        {
            _currentMedia = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged(nameof(CurrentMedia));
        }
    }

    public ICommand CloseCommand { get; }
    public ICommand PlayPauseCommand { get; }
    public ICommand MaximizeCommand { get; }
    public ICommand MuteCommand { get; }
    public ICommand MediaOpenedCommand { get; }
    public ICommand OpenPlaylistCommand { get; }
    public ICommand MediaEndedCommand { get; }

    public ICommand NextMediaCommand { get; }
    public ICommand PreviousMediaCommand { get; }
    public ICommand OpenFileCommand { get; }
    public ICommand OpenFolderCommand { get; }
    public ICommand AddFileToPlaylistCommand { get; }
    public ICommand AddFolderToPlaylistCommand { get; }
    public ICommand DeleteMediaFromPlaylistCommand { get; }
    public ICommand RepeatMediaCommand { get; }
    public ICommand RepeatCurrentMediaCommand { get; }
    
    public MainViewModel(IEventAggregator aggregator, IOService fileService, IStateService windowStateService) : base(aggregator)
    {
        MediaControlController = new MediaControlController(this);
        _fileDialogService = fileService;
        _windowStateService = windowStateService;
        _trackTitle = "";
        _isPaused = true;
        _position = TimeSpan.FromSeconds(0);
        _totalDuration = TimeSpan.Zero;
        _volume = 100;
        _volumeToFfmpeg = _volume / 100;
        _isMuted = false;
        _isPlayListOpened = false;
        _playedMediaIndex = 0;
        _playlist = new Playlist();
        _playedMediaIndex = -1;
        _selectedMediaIndex = -1;
        _currentMedia = new MediaInfo(-1);

        CloseCommand = new RelayCommand(_ => MediaControlController.Close());
        PlayPauseCommand = new RelayCommand( _ => PlayPauseSwitch(), _ => Playlist.MediaInfos.Any());
        MaximizeCommand = new RelayCommand(_ => Maximize());
        MuteCommand = new RelayCommand(_ => IsMuted = !IsMuted);
        MediaOpenedCommand = new RelayCommand(MediaOpened);
        OpenPlaylistCommand = new RelayCommand(_ => IsPlayListOpened = !IsPlayListOpened);
        MediaEndedCommand = new RelayCommand(MediaEnded);

        NextMediaCommand =
            new RelayCommand(_ => NextMediaFile(), _ => _playedMediaIndex < Playlist.MediaInfos.Count - 1);
        PreviousMediaCommand = new RelayCommand(_ => PreviousMediaFile(), _ => _playedMediaIndex > 0);

        OpenFileCommand = new RelayCommand(_ => OpenFile());
        OpenFolderCommand = new RelayCommand(_ => OpenFolder());
        AddFileToPlaylistCommand = new RelayCommand(_ => AddFileToPlaylist());
        AddFolderToPlaylistCommand = new RelayCommand(_ => AddFolderToPlaylist());

        DeleteMediaFromPlaylistCommand = new RelayCommand(_ => DeleteMediaFile(), _ => Playlist.MediaInfos.Any());
        RepeatMediaCommand = new RelayCommand(
                _ => Playlist.MediaInfos[SelectedMediaIndex].Repeat = !Playlist.MediaInfos[SelectedMediaIndex].Repeat,
                _ => Playlist.MediaInfos.Any()
                );
        RepeatCurrentMediaCommand = new RelayCommand(_ => CurrentMedia.Repeat = !CurrentMedia.Repeat,
            _ => Playlist.MediaInfos.Any());
    }

    private void PlayPauseSwitch()
    {
        if (IsPaused)
        {
            MediaControlController.Play();
            IsPaused = false;
            return;
        }

        MediaControlController.Pause();
        IsPaused = true;
    }

    /// <summary>
    /// Open media file from folder
    /// </summary>
    private void OpenFile()
    {
        string filePath = _fileDialogService.OpenFileDialog(@"C:\");
        if (string.IsNullOrWhiteSpace(filePath)) return;

        if (Playlist.MediaInfos.Any())
        {
            Playlist.MediaInfos.Clear();
            _playedMediaIndex = -1;
        }
        
        OpenMediaFileInternal(filePath);

        NextMediaFile();
        IsPaused = false;
    }

    private void OpenFolder()
    {
        string folder = _fileDialogService.OpenFolderDialog(@"C:\");
        if (string.IsNullOrWhiteSpace(folder)) return;

        if (Playlist.MediaInfos.Any())
        {
            Playlist.MediaInfos.Clear();
            _playedMediaIndex = -1;
        }
        
        var allowedExt = new[] {"mp3", "mp4", "webm", "mkv", "flv", "avi", "amv"};
        
        foreach (var f in Directory.GetFiles(folder).Where(f => allowedExt.Any(f.ToLower().EndsWith)))
        {
            OpenMediaFileInternal(f);
        }
        
        NextMediaFile();
        IsPaused = false;
    }

    /// <summary>
    /// Maximize main window
    /// </summary>
    private void Maximize()
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
    private void NextMediaFile()
    {
        if (_playedMediaIndex == Playlist.MediaInfos.Count - 1) return;
        if (_playedMediaIndex > -1)
            Playlist.MediaInfos[_playedMediaIndex].IsPlaying = false;
        
        ++_playedMediaIndex;

        Playlist.MediaInfos[_playedMediaIndex].IsPlaying = true;
        MediaInfo info = Playlist.MediaInfos[_playedMediaIndex];
        MediaControlController.OpenMediaFile(info.Path + info.Title);
    }

    /// <summary>
    /// set previous media file in container
    /// </summary>
    private void PreviousMediaFile()
    {
        if (_playedMediaIndex is < 0 or 0)
            return;
            
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
        Position = args.Info.StartTime;
        TrackTitle = $"{args.Info.MediaSource}";
        CurrentMedia = Playlist.MediaInfos[_playedMediaIndex];
        if (!IsPaused) 
            MediaControlController.Play();
    }
    
    /// <summary>
    /// Call with media opened event. Using for set next media from playlist if exist
    /// </summary>
    /// <param name="o"></param>
    private void MediaEnded(object? o)
    {
        if (!Playlist.MediaInfos.Any() || _playedMediaIndex == Playlist.MediaInfos.Count - 1 || CurrentMedia.Repeat)
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
    private void AddFileToPlaylist()
    {
        string filePath = _fileDialogService.OpenFileDialog(@"C:\");
        if (string.IsNullOrWhiteSpace(filePath)) return;
        
        OpenMediaFileInternal(filePath);
        RunMediaIfFirst();
    }
    
    private void AddFolderToPlaylist()
    {
        string folder = _fileDialogService.OpenFolderDialog(@"C:\");
        if (string.IsNullOrWhiteSpace(folder)) return;
        
        var allowedExt = new[] {"mp3", "mp4", "webm", "mkv", "flv", "avi", "amv"};
        
        foreach (var f in Directory.GetFiles(folder).Where(f => allowedExt.Any(f.ToLower().EndsWith)))
        {
            OpenMediaFileInternal(f);
            RunMediaIfFirst();
        }
    }
    
    private void DeleteMediaFile()
    {
        if (_playedMediaIndex == SelectedMediaIndex) return;
        Playlist.MediaInfos.RemoveAt(SelectedMediaIndex);
        var temp = Playlist.MediaInfos.AsEnumerable();
        Playlist.MediaInfos = new(temp);
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
        var temp = Playlist.MediaInfos.AsEnumerable();
        Playlist.MediaInfos = new(temp);

        CountPlaylistDuration();
    }

    /// <summary>
    /// Run media from playlist if it's media is only one
    /// </summary>
    private void RunMediaIfFirst()
    {
        if (Playlist.MediaInfos.Count > 1 || _playedMediaIndex > -1)
            return;
            
        NextMediaFile();
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
        OnPropertyChanged(nameof(Position));
        OnPropertyChanged(nameof(PositionSeconds));
    }
}