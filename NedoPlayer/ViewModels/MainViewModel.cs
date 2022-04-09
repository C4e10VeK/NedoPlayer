using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.IconPacks;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Services;
using NedoPlayer.Utils;

namespace NedoPlayer.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
        private readonly MediaControlService _controlService;
        
        private string _appTitle;
        private string _taskBarIcon;

        private TimeSpan _playerTimeToEnd;
        private TimeSpan _totalDuration;
        private bool _paused;

        private PackIconModernKind _playPauseKind;

        private Visibility _fullscreenVisible;

        public MediaControlService ControlService => _controlService;

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
                NotifySliderDataChanged();
            }
        }

        public double PlayerTimeToEndSeconds
        {
            get => PlayerTimeToEnd.TotalSeconds;
            set => PlayerTimeToEnd = TimeSpan.FromSeconds(value);
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

        public Visibility FullscreenVisible
        {
            get => _fullscreenVisible;
            set
            {
                _fullscreenVisible = value;
                OnPropertyChanged(nameof(FullscreenVisible));
            }
        }

        public ICommand CloseCommand { get; set; }
        public ICommand PlayPauseCommand { get; set; }

        public MainViewModel(IEventAggregator aggregator) : base(aggregator)
        {
            _controlService = new MediaControlService(this);
            _appTitle = "NedoPlayer";
            _taskBarIcon = "./Resources/img/pause.png";
            _paused = false;
            _playerTimeToEnd = TimeSpan.FromSeconds(0);
            _totalDuration = TimeSpan.Zero;
            _playPauseKind = PackIconModernKind.ControlPause;
            _fullscreenVisible = Visibility.Visible;

            CloseCommand = new RelayCommand(_controlService.Close);
            PlayPauseCommand = new RelayCommand(_controlService.PlayPause);
            
            Aggregator.GetEvent<MaximizePlayer>().Subscribe(Maximize);

            Aggregator.GetEvent<ChangeVolume>().Subscribe(vol =>  _controlService.ChangeVolume(this, vol));
        }

        private void Maximize()
        {
            if (FullscreenVisible != Visibility.Collapsed)
            {
                FullscreenVisible = Visibility.Collapsed;
                return;
            }
                
            FullscreenVisible = Visibility.Visible;
        }

        private void NotifySliderDataChanged()
        {
            OnPropertyChanged(nameof(PlayerTimeToEnd));
            OnPropertyChanged(nameof(PlayerTimeToEndSeconds));
        }
    }
}