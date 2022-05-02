using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using NedoPlayer.Controllers;
using NedoPlayer.Models;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Services;
using NedoPlayer.Utils;
using NedoPlayer.Views;
using Application = System.Windows.Application;

#pragma warning disable CS8618

namespace NedoPlayer.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    public MediaControlController MediaControlController { get; }

    private MediaControlModel _mediaControlModel;

    public MediaControlModel MediaControlModel
    {
        get => _mediaControlModel;
        set
        {
            _mediaControlModel = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged(nameof(MediaControlModel));
        }
    }

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

    private bool _isPlayListOpened;
    public bool IsPlayListOpened
    {
        get => _isPlayListOpened;
        set
        {
            _isPlayListOpened = value && !_isPlaylistOpenedInWindow;
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

    public ICommand CloseCommand { get; private set; }
    public ICommand PlayPauseCommand { get; private set; }
    public ICommand MaximizeCommand { get; private set; }
    public ICommand MuteCommand { get; private set; }
    public ICommand MediaOpenedCommand { get; private set; }
    public ICommand OpenPlaylistCommand { get; private set; }
    public ICommand MediaEndedCommand { get; private set; }
    public ICommand NextMediaCommand { get; private set; }
    public ICommand PreviousMediaCommand { get; private set; }
    public ICommand OpenFileCommand { get; private set; }
    public ICommand OpenFolderCommand { get; private set; }
    public ICommand AddFileToPlaylistCommand { get; private set; }
    public ICommand AddFolderToPlaylistCommand { get; private set; }
    public ICommand DeleteMediaFromPlaylistCommand { get; private set; }
    public ICommand RepeatMediaCommand { get; private set; }
    public ICommand RepeatCurrentMediaCommand { get; private set; }
    public ICommand PlusVolumeCommand { get; private set; }
    public ICommand MinusVolumeCommand { get; private set; }
    public ICommand OpenAboutCommand { get; private set; }
    public ICommand OpenPlaylistInWindowCommand { get; private set; }
    public ICommand ClearPlaylistCommand { get; private set; }
    public ICommand ShowHelpCommand { get; private set; }

    private readonly IOService _fileDialogService;
    private readonly IStateService _windowStateService;
    private readonly IWindowService _windowService;
    private int _playedMediaIndex;
    private bool _isPlaylistOpenedInWindow;

    private readonly SubscriptionToken _closePlaylistWindowEventToken;
    private readonly SubscriptionToken _deleteMediaEventToken;
    private readonly SubscriptionToken _repeatMediaEventToken;
    private readonly SubscriptionToken _addMediaFileEventToken;
    private readonly SubscriptionToken _addFolderEventToken;
    private readonly SubscriptionToken _clearPlaylistEventToken;

    public MainViewModel(IEventAggregator aggregator, IOService fileService, IStateService windowStateService,
        IWindowService windowService, IConfigFileService configFileService) : base(aggregator)
    {
        _fileDialogService = fileService;
        _windowStateService = windowStateService;
        _windowService = windowService;
        MediaControlController = new MediaControlController(this);
        _mediaControlModel = new MediaControlModel(configFileService, MediaControlController);
        
        _trackTitle = "";
        _isPlayListOpened = false;
        _isPlaylistOpenedInWindow = false;
        _playedMediaIndex = 0;
        _playlist = new Playlist();
        _playedMediaIndex = -1;
        _selectedMediaIndex = -1;
        _currentMedia = new MediaInfo(-1);

        _closePlaylistWindowEventToken = Aggregator.GetEvent<ClosePlaylistWindowEvent>().Subscribe(() => _isPlaylistOpenedInWindow = false);
        _deleteMediaEventToken = Aggregator.GetEvent<DeleteMediaEvent>().Subscribe(DeleteMediaFile);
        _repeatMediaEventToken = Aggregator.GetEvent<RepeatMediaEvent>().Subscribe(RepeatSelectedMedia);
        _addMediaFileEventToken = Aggregator.GetEvent<AddMediaFileEvent>().Subscribe(AddFileToPlaylist);
        _addFolderEventToken = Aggregator.GetEvent<AddFolderEvent>().Subscribe(AddFolderToPlaylist);
        _clearPlaylistEventToken = Aggregator.GetEvent<ClearPlaylistEvent>().Subscribe(ClearPlaylist);

        InitCommands();
    }

    private void InitCommands()
    {
        CloseCommand = new RelayCommand(Close);
        PlayPauseCommand = new RelayCommand( _ => PlayPauseSwitch(), _ => Playlist.MediaInfos.Any());
        MaximizeCommand = new RelayCommand(_ => Maximize());
        MuteCommand = new RelayCommand(_ => MediaControlModel.IsMuted = !MediaControlModel.IsMuted);
        MediaOpenedCommand = new RelayCommand(MediaOpened);
        OpenPlaylistCommand =
            new RelayCommand(_ => IsPlayListOpened = !IsPlayListOpened, _ => !_isPlaylistOpenedInWindow);
        MediaEndedCommand = new RelayCommand(MediaEnded);

        NextMediaCommand =
            new RelayCommand(_ => NextMediaFile(), _ => _playedMediaIndex < Playlist.MediaInfos.Count - 1);
        PreviousMediaCommand = new RelayCommand(_ => PreviousMediaFile(), _ => _playedMediaIndex > 0);

        OpenFileCommand = new RelayCommand(_ => OpenFile());
        OpenFolderCommand = new RelayCommand(_ => OpenFolder());
        AddFileToPlaylistCommand = new RelayCommand(_ => AddFileToPlaylist());
        AddFolderToPlaylistCommand = new RelayCommand(_ => AddFolderToPlaylist());

        DeleteMediaFromPlaylistCommand = new RelayCommand(_ => DeleteMediaFile(SelectedMediaIndex), _ => Playlist.MediaInfos.Any());
        RepeatMediaCommand = new RelayCommand(
            _ => RepeatSelectedMedia(SelectedMediaIndex),
            _ => Playlist.MediaInfos.Any()
        );
        RepeatCurrentMediaCommand = new RelayCommand(_ => CurrentMedia.IsRepeat = !CurrentMedia.IsRepeat,
            _ => Playlist.MediaInfos.Any());

        PlusVolumeCommand = new RelayCommand(_ => MediaControlModel.Volume += 5, _ => !MediaControlModel.IsMuted);
        MinusVolumeCommand = new RelayCommand(_ => MediaControlModel.Volume -= 5, _ => !MediaControlModel.IsMuted);
        OpenAboutCommand = new RelayCommand(_ => _windowService.OpenDialogWindow<AboutWindow>(Application.Current.MainWindow));

        OpenPlaylistInWindowCommand = new RelayCommand(_ => OpenPlaylistInWindow());
        ClearPlaylistCommand = new RelayCommand(_ => ClearPlaylist(), _ => Playlist.MediaInfos.Any());

        ShowHelpCommand = new RelayCommand(_ =>
            Help.ShowHelp(null, @"./Resources/NedoPlayerHelp.chm"));
    }

    private void Close(object? s)
    {
        if (s is not Window wnd) return;
        
        Aggregator.GetEvent<ClosePlaylistWindowEvent>().Unsubscribe(_closePlaylistWindowEventToken);
        Aggregator.GetEvent<DeleteMediaEvent>().Unsubscribe(_deleteMediaEventToken);
        Aggregator.GetEvent<RepeatMediaEvent>().Unsubscribe(_repeatMediaEventToken);
        Aggregator.GetEvent<AddMediaFileEvent>().Unsubscribe(_addMediaFileEventToken);
        Aggregator.GetEvent<AddFolderEvent>().Unsubscribe(_addFolderEventToken);
        Aggregator.GetEvent<ClearPlaylistEvent>().Unsubscribe(_clearPlaylistEventToken);

        MediaControlController.CloseMedia();
        wnd.Close();
    }

    private void PlayPauseSwitch()
    {
        if (MediaControlModel.IsPaused)
        {
            MediaControlController.Play();
            MediaControlModel.IsPaused = false;
            return;
        }

        MediaControlController.Pause();
        MediaControlModel.IsPaused = true;
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
        MediaControlModel.IsPaused = false;
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
        MediaControlModel.IsPaused = false;
    }

    /// <summary>
    /// Maximize main window
    /// </summary>
    private void Maximize()
    {
        if (Application.Current.MainWindow is not MetroWindow wnd) return;

        if (!MediaControlModel.IsFullscreen)
        {
            _windowStateService.SaveState(wnd);
                
            wnd.WindowState = WindowState.Maximized;
            wnd.WindowStyle = WindowStyle.None;
            wnd.UseNoneWindowStyle = true;
            wnd.IgnoreTaskbarOnMaximize = true;

            MediaControlModel.IsFullscreen = true;
            MediaControlController.Maximize(MediaControlModel.IsFullscreen);
            return;
        }
            
        _windowStateService.LoadState(wnd);
        wnd.IgnoreTaskbarOnMaximize = false;
        wnd.UseNoneWindowStyle = false;
        wnd.ShowTitleBar = true;

        MediaControlModel.IsFullscreen = false;
        MediaControlController.Maximize(MediaControlModel.IsFullscreen);
    }

    /// <summary>
    /// Set next media file in container
    /// </summary>
    private void NextMediaFile()
    {
        if (_playedMediaIndex >= Playlist.MediaInfos.Count - 1) return;
        if (_playedMediaIndex > -1)
        {
            Playlist[_playedMediaIndex].IsPlaying = false;
            Playlist[_playedMediaIndex].IsRepeat = false;
        }

        ++_playedMediaIndex;

        Playlist[_playedMediaIndex].IsPlaying = true;
        MediaInfo info = Playlist[_playedMediaIndex];
        MediaControlController.OpenMediaFile(info.Path + info.Title);
    }

    /// <summary>
    /// set previous media file in container
    /// </summary>
    private void PreviousMediaFile()
    {
        if (_playedMediaIndex is < 0 or 0)
            return;
            
        Playlist[_playedMediaIndex].IsPlaying = false;
        Playlist[_playedMediaIndex].IsRepeat = false;

        _playedMediaIndex--;

        Playlist[_playedMediaIndex].IsPlaying = true;
        MediaInfo info = Playlist[_playedMediaIndex];
        MediaControlController.OpenMediaFile(info.Path + info.Title);
    }

    /// <summary>
    /// Call with media opened event. using for set value for media element control
    /// </summary>
    /// <param name="o">Args from media opened event</param>
    private void MediaOpened(object? o)
    {
        if (o is not RoutedEventArgs {Source: MediaElement me})
            return;

        CurrentMedia = Playlist[_playedMediaIndex];
        MediaControlModel.TotalDuration = me.NaturalDuration.TimeSpan;
        MediaControlModel.Position = TimeSpan.Zero;
        TrackTitle = CurrentMedia.Title;

        if (!MediaControlModel.IsPaused) 
            MediaControlController.Play();
    }
    
    /// <summary>
    /// Call with media opened event. Using for set next media from playlist if exist
    /// </summary>
    /// <param name="o"></param>
    private void MediaEnded(object? o)
    {
        if (!Playlist.MediaInfos.Any() || _playedMediaIndex == Playlist.MediaInfos.Count - 1)
            return;

        if (CurrentMedia.IsRepeat)
        {
            MediaControlModel.PositionSeconds = 0;
            return;
        }

        NextMediaFile();
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
    
    private void DeleteMediaFile(int selectedIndex)
    {
        if (_playedMediaIndex == selectedIndex) return;
        Playlist.MediaInfos.RemoveAt(selectedIndex);
        var temp = Playlist.MediaInfos.AsEnumerable();
        Playlist.MediaInfos = new ObservableCollection<MediaInfo>(temp);
        
        if (_playedMediaIndex <= selectedIndex) return;
        --_playedMediaIndex;
    }
    
    private void RepeatSelectedMedia(int selectedIndex) =>
        Playlist[selectedIndex].IsRepeat = !Playlist[selectedIndex].IsRepeat;
    
    private void OpenPlaylistInWindow()
    {
        _windowService.OpenWindow<PlaylistWindow>(Application.Current.MainWindow);
        IsPlayListOpened = false;
        _isPlaylistOpenedInWindow = true;
        Aggregator.GetEvent<PlaylistUpdateEvent>().Publish(Playlist);
    }

    private void ClearPlaylist()
    {
        if (!MediaControlModel.IsPaused)
            PlayPauseSwitch();

        MediaControlController.CloseMedia();
        MediaControlModel.Position = TimeSpan.Zero;
        MediaControlModel.TotalDuration = TimeSpan.Zero;
        _playedMediaIndex = -1;
        Playlist.MediaInfos.Clear();
        TrackTitle = "";
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

        MediaControlModel.IsPaused = false;
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
}