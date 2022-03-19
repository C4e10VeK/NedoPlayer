using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.IconPacks;
using NedoPlayer.Utils;

namespace NedoPlayer.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
        private string _appTitle;
        private string _taskBarIcon;

        private TimeSpan _playerTimeToEnd;
        private TimeSpan _totalDuration;
        private bool _paused;

        private PackIconModernKind _playPauseKind;

        public event EventHandler? CloseRequested;
        public event EventHandler? PlayPauseRequested;
        public event EventHandler<string>? NextRequested;
        public event EventHandler<string>? PrevRequested;

        public string AppTitle
        {
            get => _appTitle;
            set
            {
                _appTitle = value;
                OnPropertyChanged(nameof(AppTitle));
            }
        }

        public string TaskBarIcon
        {
            get => _taskBarIcon;
            set
            {
                _taskBarIcon = value;
                OnPropertyChanged(nameof(TaskBarIcon));
            }
        }

        public TimeSpan PlayerTimeToEnd
        {
            get => _playerTimeToEnd;
            set
            {
                _playerTimeToEnd = value;
                OnPropertyChanged(nameof(PlayerTimeToEnd));
            }
        }

        public double PlayerTimeToEndSeconds
        {
            get => _playerTimeToEnd.TotalSeconds;
            set
            {
                _playerTimeToEnd = TimeSpan.FromSeconds(value);
                OnPropertyChanged(nameof(PlayerTimeToEndSeconds));
                OnPropertyChanged(nameof(PlayerTimeToEnd));
            }
        }

        public TimeSpan TotalDuration
        {
            get => _totalDuration;
            set
            {
                _totalDuration = value;
                OnPropertyChanged(nameof(TotalDuration));
            }
        }

        public PackIconModernKind PlayPauseKind
        {
            get => _playPauseKind;
            set
            {
                _playPauseKind = value;
                OnPropertyChanged(nameof(PlayPauseKind));
            }
        }

        public bool Paused
        {
            get => _paused;
            set => _paused = value;
        }

        public ICommand CloseCommand { get; set; }
        public ICommand PlayPauseCommand { get; set; }

        public MainViewModel()
        {
            _appTitle = "NedoPlayer";
            _taskBarIcon = "./Resources/pause.png";
            _paused = false;
            _playerTimeToEnd = TimeSpan.FromSeconds(0);
            _totalDuration = TimeSpan.Zero;
            _playPauseKind = PackIconModernKind.ControlPause;

            CloseCommand = new RelayCommand(Close);
            PlayPauseCommand = new RelayCommand(PlayPause);
        }

        private void PlayPause(object? s)
        {
            PlayPauseRequested?.Invoke(this, EventArgs.Empty);
            if (!Paused)
            {
                PlayPauseKind = PackIconModernKind.ControlPlay;
                TaskBarIcon = "./Resources/play.png";
            }
            else
            {
                PlayPauseKind = PackIconModernKind.ControlPause;
                TaskBarIcon = "./Resources/pause.png";
            }
        }

        private void Next(object? s)
        {
            NextRequested?.Invoke(this, string.Empty);
        }

        private void Close(object? s) => CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}