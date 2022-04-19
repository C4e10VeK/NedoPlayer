using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
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
        public MediaControlController ControlController { get; }

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

        public MainViewModel(IEventAggregator aggregator) : base(aggregator)
        {
            ControlController = new MediaControlController(this);
            _trackTitle = "";
            _isPaused = true;
            _playerTimeToEnd = TimeSpan.FromSeconds(0);
            _totalDuration = TimeSpan.Zero;
            _volume = 100;
            _isMuted = false;
            _isPlayListOpened = false;
            _playlist = new Playlist
            {
                MediaInfos = new ObservableCollection<MediaInfo>
                {
                    new() {Duration = TimeSpan.FromSeconds(635), GroupId = 0, Path = "no", Title = "Первый"},
                    new() {Duration = TimeSpan.FromSeconds(635), GroupId = 1, Path = "no", Title = "Второй"},
                    new() {Duration = TimeSpan.FromSeconds(635), GroupId = 0, Path = "no", Title = "Третий"}
                },
                TotalDuration = TimeSpan.FromSeconds(635 * 3)
            };

            CloseCommand = new RelayCommand(ControlController.Close);
            PlayPauseCommand = new RelayCommand(ControlController.PlayPause);
            MaximizeCommand = new RelayCommand(Maximize);
            MuteCommand = new RelayCommand(_ => IsMuted = !IsMuted);
            MediaOpenedCommand = new RelayCommand(MediaOpened);
            OpenPlaylistCommand = new RelayCommand(_ => IsPlayListOpened = !IsPlayListOpened);
            SeekStartCommand = new RelayCommand(o =>
            {
                if (IsPaused) return;
                ControlController.PlayPause(o);
            });
            SeekEndCommand = new RelayCommand(ControlController.PlayPause);
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
                ControlController.Maximize(IsFullscreen);
                return;
            }
            
            WindowStateService.LoadCurrentWindowState(ref wnd);
            wnd.UseNoneWindowStyle = false;
            wnd.ShowTitleBar = true;

            IsFullscreen = false;
            ControlController.Maximize(IsFullscreen);
        }

        private void MediaOpened(object? o)
        {
            Debug.Write(o?.GetType().Name);
            if (o is not MediaOpenedEventArgs args)
                return;
            
            TotalDuration = args.Info.Duration;
            TrackTitle = $"{args.Info.MediaSource}";
            IsPaused = false;
        }

        private void NotifySliderDataChanged()
        {
            OnPropertyChanged(nameof(PlayerTimeToEnd));
            OnPropertyChanged(nameof(PlayerTimeToEndSeconds));
        }
    }
}