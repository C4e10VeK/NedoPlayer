using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Shell;
using NedoPlayer.Controllers;
using NedoPlayer.Models;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Services;
using NedoPlayer.Utils;
using Unosquare.FFME.Common;
using MediaInfo = NedoPlayer.Models.MediaInfo;

namespace NedoPlayer.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
        public MediaControlController MediaControlController { get; }
        private readonly FileService _fileDialogService;
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
                VolumeToFmpeg = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        public double VolumeToFmpeg
        {
            get => _volume / 100;
            set
            {
                _volume = value;
                OnPropertyChanged(nameof(VolumeToFmpeg));
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

        public ICommand OpenFileCommand { get; }
        public ICommand NextMediaCommand { get; }
        public ICommand PreviousMediaCommand { get; }

        public MainViewModel(IEventAggregator aggregator) : base(aggregator)
        {
            MediaControlController = new MediaControlController(this);
            _fileDialogService = new FileService();
            _trackTitle = "";
            _isPaused = true;
            _playerTimeToEnd = TimeSpan.FromSeconds(0);
            _totalDuration = TimeSpan.Zero;
            _volume = 100;
            _isMuted = false;
            _isPlayListOpened = false;
            _playedMediaIndex = 0;
            _playlist = new Playlist();
            _playedMediaIndex = -1;

            CloseCommand = new RelayCommand(MediaControlController.Close);
            PlayPauseCommand = new RelayCommand(_ => MediaControlController.PlayPause());
            MaximizeCommand = new RelayCommand(Maximize);
            MuteCommand = new RelayCommand(_ => IsMuted = !IsMuted);
            MediaOpenedCommand = new RelayCommand(MediaOpened);
            OpenPlaylistCommand = new RelayCommand(_ => IsPlayListOpened = !IsPlayListOpened);
            SeekStartCommand = new RelayCommand(_ =>
            {
                if (IsPaused) return;
                MediaControlController.PlayPause();
            });
            SeekEndCommand = new RelayCommand(_ => MediaControlController.PlayPause());
            OpenFileCommand = new RelayCommand(OpenMediaFile);

            NextMediaCommand = new RelayCommand(NextMediaFile, _ => _playedMediaIndex < Playlist.MediaInfos.Count);
            PreviousMediaCommand = new RelayCommand(PreviousMediaFile, _ => _playedMediaIndex > 0);
        }

        private void Maximize(object? s)
        {
            if (Application.Current.MainWindow is not MetroWindow wnd) return;

            if (!_isFullscreen)
            {
                WindowStateService.SaveCurrentWindowState(wnd);
            
                wnd.WindowState = WindowState.Maximized;
                wnd.WindowStyle = WindowStyle.None;
                wnd.UseNoneWindowStyle = true;

                IsFullscreen = true;
                MediaControlController.Maximize(IsFullscreen);
                return;
            }
            
            WindowStateService.LoadCurrentWindowState(ref wnd);
            wnd.UseNoneWindowStyle = false;
            wnd.ShowTitleBar = true;

            IsFullscreen = false;
            MediaControlController.Maximize(IsFullscreen);
        }

        private void NextMediaFile(object? o)
        {

        }

        private void PreviousMediaFile(object? o)
        {

        }

        private void MediaOpened(object? o)
        {
            if (o is not MediaOpenedEventArgs args)
                return;
            
            TotalDuration = args.Info.Duration;
            TrackTitle = $"{args.Info.MediaSource}";
            IsPaused = false;
        }

        private void OpenMediaFile(object? o)
        {
            string filePath = _fileDialogService.OpenFileDialog(this, @"C:\");
            OpenMediaFileInternal(filePath);
            if (Playlist.MediaInfos.Count != 1 || _playedMediaIndex > -1) return;
            var path = Playlist.MediaInfos.First().Path + Playlist.MediaInfos.First().Title;
            MediaControlController.OpenMediaFile(path);
            Playlist.MediaInfos.First().IsPlaying = true;
            ++_playedMediaIndex;
        }

        public void OpenMediaFileInternal(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;
            string title = Path.GetFileName(filePath);


            var tempMediaList = new ObservableCollection<MediaInfo>(Playlist.MediaInfos.OrderBy(x => x));
            Debug.Assert(Playlist != null, "Playlist != null");
            var groupList = Playlist?.MediaInfos.Where(x => x.Path == filePath.Replace(title, "")).ToList();
            var groupId = tempMediaList.Count == 0 ?
                0 : groupList?.Count <= 0 ? tempMediaList.Last().GroupId + 1 : groupList?.First().GroupId;

            using var shell = ShellObject.FromParsingName(filePath);
            var prop = shell.Properties.System.Media.Duration;
            var duration = prop.Value ?? 0;
            var mediaAdd = new MediaInfo(groupId.GetValueOrDefault(), filePath.Replace(title, ""), title, TimeSpan.FromTicks((long)duration));
            Playlist?.MediaInfos.Add(mediaAdd);
            CountPlaylistDuration();
        }

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
}