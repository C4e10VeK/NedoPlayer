﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
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
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

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
    public ICommand MinimizeCommand { get; private set; }
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
    public ICommand PlaySelectedCommand { get; private set; }
    public ICommand OpenFileInfoCommand { get; private set; }
    public ICommand PlayCommand { get; private set; }
    public ICommand PauseCommand { get; private set; }
    public ICommand DropCommand { get; private set; }
    public ICommand DragOverItemCommand { get; private set; }
    public ICommand DropItemCommand { get; private set; }
    
    public ICommand OpenPlaylistFileCommand { get; private set; }
    public ICommand SavePlaylistCommand { get; private set; }

    private readonly IFileService _fileDialogService;
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
    private readonly SubscriptionToken _playSelectedEventToken;
    private readonly SubscriptionToken _dropEventToken;
    private readonly SubscriptionToken _dropItemEventToken;
    private readonly SubscriptionToken _dragOverItemEventToken;

    public MainViewModel(IEventAggregator aggregator, IFileService fileService, IStateService windowStateService,
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
        _playSelectedEventToken = Aggregator.GetEvent<PlaySelectedEvent>().Subscribe(PlaySelected);
        _dropEventToken = Aggregator.GetEvent<DropEvent>().Subscribe(DropFileOpen);
        _dropItemEventToken = Aggregator.GetEvent<DropItemEvent>().Subscribe(DropItem);
        _dragOverItemEventToken = Aggregator.GetEvent<DragOverItemEvent>().Subscribe(DragOverItem);
        
        InitCommands();
    }

    public void OpenPlaylistFile(string filePath)
    {
        Playlist = _fileDialogService.OpenPlaylist(filePath);
        UpdateGroupId();
        Playlist.MediaInfos = new ObservableCollection<MediaInfo>(Playlist.MediaInfos);
        CountPlaylistDuration();
        Aggregator.GetEvent<PlaylistUpdateEvent>().Publish(Playlist);
        NextMediaFile();
    }

    private void InitCommands()
    {
        CloseCommand = new RelayCommand(Close);
        PlayPauseCommand = new RelayCommand( _ => PlayPauseSwitch(), _ => Playlist.MediaInfos.Any());
        MaximizeCommand = new RelayCommand(_ => Maximize());
        MinimizeCommand = new RelayCommand(_ => Maximize(), _ => MediaControlModel.IsFullscreen);
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

        PlaySelectedCommand = new RelayCommand(o =>
        {
            if (o is not MouseButtonEventArgs) return;
            PlaySelected(SelectedMediaIndex);
        }, _ => Playlist.MediaInfos.Any());

        OpenFileInfoCommand = new RelayCommand(_ =>
        {
            var filePath = $"{CurrentMedia.Path}{CurrentMedia.Title}";
            FilePropertiesWindow.Show(filePath);
        }, _ => Playlist.MediaInfos.Any());

        PlayCommand = new RelayCommand(_ => MediaControlController.Play(), _ => Playlist.MediaInfos.Any());
        PauseCommand = new RelayCommand(_ => MediaControlController.Pause(), _ => Playlist.MediaInfos.Any());

        DropCommand = new RelayCommand(DropFileOpen);

        DragOverItemCommand = new RelayCommand(DragOverItem);
        
        DropItemCommand = new RelayCommand(DropItem);

        OpenPlaylistFileCommand = new RelayCommand(_ => OpenPlaylistFile());

        SavePlaylistCommand =
            new RelayCommand(
                _ => _fileDialogService.SavePlaylist(Playlist,
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), _ => Playlist.MediaInfos.Any());
    }

    private void OpenPlaylistFile()
    {
        string filePath =
            _fileDialogService.OpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                @"nypl files (*.nypl)|*.nypl");
        if (!File.Exists(filePath)) return;

        Playlist.MediaInfos.Clear();
        _playedMediaIndex = -1;
        Playlist = _fileDialogService.OpenPlaylist(filePath);
        UpdateGroupId();
        Playlist.MediaInfos = new ObservableCollection<MediaInfo>(Playlist.MediaInfos);
        CountPlaylistDuration();
        Aggregator.GetEvent<PlaylistUpdateEvent>().Publish(Playlist);
        NextMediaFile();
        MediaControlModel.IsPaused = false;
    }

    private void DropItem(object? o)
    {
        if (o is not IDropInfo dropInfo) return;
        
        var sourceItem = dropInfo.Data as MediaInfo;
        var targetItem = dropInfo.TargetItem as MediaInfo;

        if (sourceItem is null && targetItem is null) return;

        int sourceIndex = Playlist.MediaInfos.IndexOf(sourceItem!);
        int targetIndex = Playlist.MediaInfos.IndexOf(targetItem!);
        if (targetIndex < 0) return;

        if (sourceIndex < targetIndex)
        {
            Playlist.MediaInfos.Insert(targetIndex + 1, sourceItem!);
            Playlist.MediaInfos.RemoveAt(sourceIndex);
            
            if (_playedMediaIndex > 0) --_playedMediaIndex;
            if (Playlist[targetIndex].IsPlaying) _playedMediaIndex = targetIndex;
        }
        else if (sourceIndex > targetIndex)
        {
            int removeIndex = sourceIndex + 1;
            if (Playlist.MediaInfos.Count < removeIndex) return;
            
            Playlist.MediaInfos.Insert(targetIndex, sourceItem!);
            Playlist.MediaInfos.RemoveAt(removeIndex);
            ++_playedMediaIndex;
            if (Playlist[targetIndex].IsPlaying) _playedMediaIndex = targetIndex;
        }
        
        UpdateGroupId();
        Playlist.MediaInfos = new ObservableCollection<MediaInfo>(Playlist.MediaInfos);
    }

    private void UpdateGroupId()
    {
        for (int index = 0, j = 0; index < Playlist.MediaInfos.Count; index++)
        {
            switch (index)
            {
                case 0:
                    Playlist.MediaInfos[index].GroupId = j;
                    break;
                case > 0 when Playlist[index - 1].Path == Playlist.MediaInfos[index].Path:
                    Playlist.MediaInfos[index].GroupId = Playlist[index - 1].GroupId;
                    break;
                case > 0 when Playlist[index - 1].Path != Playlist.MediaInfos[index].Path:
                    ++j;
                    Playlist.MediaInfos[index].GroupId = j;
                    break;
            }
        }
    }

    private void DragOverItem(object? o)
    {
        if (o is not IDropInfo dropInfo) return;

        var sourceItem = dropInfo.Data as MediaInfo;
        var targetItem = dropInfo.TargetItem as MediaInfo;

        if (sourceItem is null && targetItem is null) return;

        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Move;
    }

    private void DropFileOpen(object? o)
    {
        if (o is not DragEventArgs args) return;

        string[] allowedExt = {"mp3", "mp4", "webm", "mkv", "wav", "ogg", "oga", "mogg"};
        
        // ReSharper disable once AssignNullToNotNullAttribute
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        var files = (string[]) args.Data.GetData(DataFormats.FileDrop) ?? Array.Empty<string>();

        if (files[0].ToLower().EndsWith("nypl"))
        {
            Playlist.MediaInfos.Clear();
            _playedMediaIndex = -1;
            Playlist = _fileDialogService.OpenPlaylist(files[0]);
            UpdateGroupId();
            Playlist.MediaInfos = new ObservableCollection<MediaInfo>(Playlist.MediaInfos);
            CountPlaylistDuration();
            Aggregator.GetEvent<PlaylistUpdateEvent>().Publish(Playlist);
            NextMediaFile();
            MediaControlModel.IsPaused = false;
            return;
        }
        
        foreach (var f in files)
        {
            if (Directory.Exists(f))
            {
                foreach (var fd in Directory.GetFiles(f).Where(s => allowedExt.Any(s.ToLower().EndsWith)))
                {
                    OpenMediaFileInternal(fd);
                    RunMediaIfFirst();
                }

                continue;
            }

            if (File.Exists(f) && allowedExt.Any(f.ToLower().EndsWith))
            {
                OpenMediaFileInternal(f);
                RunMediaIfFirst();
            }
        }
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
        Aggregator.GetEvent<PlaySelectedEvent>().Unsubscribe(_playSelectedEventToken);
        Aggregator.GetEvent<DropEvent>().Unsubscribe(_dropEventToken);
        Aggregator.GetEvent<DropItemEvent>().Unsubscribe(_dropItemEventToken);
        Aggregator.GetEvent<DragOverItemEvent>().Unsubscribe(_dragOverItemEventToken);

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
        string filePath = _fileDialogService.OpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
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
        string folder = _fileDialogService.OpenFolderDialog(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        if (string.IsNullOrWhiteSpace(folder)) return;

        if (Playlist.MediaInfos.Any())
        {
            Playlist.MediaInfos.Clear();
            _playedMediaIndex = -1;
        }

        string[] allowedExt = {"mp3", "mp4", "webm", "mkv", "wav", "ogg", "oga", "mogg"};
        
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
        string filePath = _fileDialogService.OpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        if (string.IsNullOrWhiteSpace(filePath)) return;
        
        OpenMediaFileInternal(filePath);
        RunMediaIfFirst();
    }
    
    private void AddFolderToPlaylist()
    {
        string folder = _fileDialogService.OpenFolderDialog(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        if (string.IsNullOrWhiteSpace(folder)) return;
        
        string[] allowedExt = {"mp3", "mp4", "webm", "mkv", "wav", "ogg", "oga", "mogg"};

        
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
        CountPlaylistDuration();
        
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
        Playlist.TotalDuration = TimeSpan.Zero;
    }

    private void PlaySelected(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= Playlist.MediaInfos.Count ||
            selectedIndex == _playedMediaIndex) return;
        
        Playlist[_playedMediaIndex].IsPlaying = false;
        Playlist[_playedMediaIndex].IsRepeat = false;

        _playedMediaIndex = selectedIndex;

        Playlist[_playedMediaIndex].IsPlaying = true;
        MediaInfo info = Playlist[_playedMediaIndex];
        MediaControlController.OpenMediaFile(info.Path + info.Title);
    }

    /// <summary>
    /// Open internal media file
    /// </summary>
    /// <param name="filePath"></param>
    internal void OpenMediaFileInternal(string filePath)
    {
        // TODO: Rewrite this trash
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return;
        string title = Path.GetFileName(filePath);
        string path = filePath.Replace(title, "");

        // Sorting medias by group
        var tempMediaList = Playlist.MediaInfos.OrderBy(x => x).ToList();
        Debug.Assert(tempMediaList != null, "tempMediaList != null");
        // get group medias by path
        List<MediaInfo> groupList = Playlist.MediaInfos.Where(x => x.Path == filePath.Replace(title, "")).ToList();
        if (Playlist.MediaInfos.Any(x => x.Path == path && x.Title == title))
            return;
        
        // Get media duration 
        using ShellObject shell = ShellObject.FromParsingName(filePath);
        ShellProperty<ulong?> prop = shell.Properties.System.Media.Duration;
        ulong duration = prop.Value ?? 0;
        
        MediaInfo mediaAdd = new MediaInfo(0, filePath.Replace(title, ""), title, TimeSpan.FromTicks((long)duration));

        Playlist.MediaInfos.Add(mediaAdd);
        
        UpdateGroupId();
        Playlist.MediaInfos = new ObservableCollection<MediaInfo>(Playlist.MediaInfos);
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
        Playlist.TotalDuration = TimeSpan.Zero;
        foreach (var mediaInfo in Playlist.MediaInfos)
            Playlist.TotalDuration += mediaInfo.Duration.GetValueOrDefault(TimeSpan.Zero);
    }
}