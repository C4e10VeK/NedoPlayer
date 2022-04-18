using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using NedoPlayer.Controllers;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Utils;
using Unosquare.FFME.Common;

namespace NedoPlayer.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
        public MediaControlController ControlController { get; }

        private WindowState _wndState;
        private WindowStyle _wndStyle;
        private double _wndWidth;
        private double _wndHeight;

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

        private string _taskBarIcon;
        public string TaskBarIcon
        {
            get => _taskBarIcon;
            set
            {
                _taskBarIcon = value;
                OnPropertyChanged(nameof(TaskBarIcon));
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

        private Visibility _fullscreenVisible;
        public Visibility FullscreenVisible
        {
            get => _fullscreenVisible;
            set
            {
                _fullscreenVisible = value;
                OnPropertyChanged(nameof(FullscreenVisible));
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
            get => _volume / 100;
            set
            {
                _volume = value;
                OnPropertyChanged(nameof(Volume));
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

        public ICommand CloseCommand { get; }
        public ICommand PlayPauseCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand MuteCommand { get; }
        public ICommand MediaOpenedCommand { get; }

        public MainViewModel(IEventAggregator aggregator) : base(aggregator)
        {
            ControlController = new MediaControlController(this);
            _trackTitle = "";
            _taskBarIcon = "./Resources/img/pause.png";
            _isPaused = true;
            _playerTimeToEnd = TimeSpan.FromSeconds(0);
            _totalDuration = TimeSpan.Zero;
            _fullscreenVisible = Visibility.Visible;
            _volume = 100;
            _isMuted = false;

            CloseCommand = new RelayCommand(ControlController.Close);
            PlayPauseCommand = new RelayCommand(ControlController.PlayPause);
            MaximizeCommand = new RelayCommand(Maximize);
            MuteCommand = new RelayCommand(_ => IsMuted = !IsMuted);
            MediaOpenedCommand = new RelayCommand(MediaOpened);
        }

        private void Maximize(object? s)
        {
            if (Application.Current.MainWindow is not MetroWindow wnd) return;

            if (!_isFullscreen)
            {
                _wndState = wnd.WindowState;
                _wndStyle = wnd.WindowStyle;
                _wndWidth = wnd.Width;
                _wndHeight = wnd.Height;
            
                wnd.WindowState = WindowState.Maximized;
                wnd.WindowStyle = WindowStyle.None;
                wnd.UseNoneWindowStyle = true;
                FullscreenVisible = Visibility.Collapsed;

                IsFullscreen = true;
                ControlController.Maximize(IsFullscreen);
                return;
            }

            wnd.WindowState = _wndState;
            wnd.WindowStyle = _wndStyle;
            wnd.UseNoneWindowStyle = false;
            wnd.ShowTitleBar = true;
            wnd.Width = _wndWidth;
            wnd.Height = _wndHeight;
            FullscreenVisible = Visibility.Visible;

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