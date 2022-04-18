using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Utils;

namespace NedoPlayer.ViewModels;

public class ControlViewModel : BaseViewModel
{
    public ICommand MaximizeCommand { get; }
    public ICommand MuteCommand { get; }
    
    private WindowState _wndState;
    private WindowStyle _wndStyle;
    private double _wndWidth;
    private double _wndHeight;

    private bool _isMuted;
    private bool _isMaximized;

    private PackIconModernKind _volumeKind;

    public PackIconModernKind VolumeKind
    {
        get => _volumeKind;
        set
        {
            _volumeKind = value;
            OnPropertyChanged(nameof(VolumeKind));
        }
    }
    
    private double _prevVolume;

    private double _volume;
    public double Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            OnPropertyChanged(nameof(Volume));
            Aggregator.GetEvent<ChangeVolume>().Publish(_volume / 100);
            VolumeKind = value switch
            {
                > 75 => PackIconModernKind.Sound3,
                <= 75 and > 50 => PackIconModernKind.Sound2,
                <= 50 and > 25 => PackIconModernKind.Sound1,
                <= 25 and > 0 => PackIconModernKind.Sound0,
                _ => PackIconModernKind.SoundMute
            };
        }
    }

    public ControlViewModel(IEventAggregator aggregator) : base(aggregator)
    {
        MaximizeCommand = new RelayCommand(Maximize);
        MuteCommand = new RelayCommand(Mute);
        _volumeKind = PackIconModernKind.Sound3;
        _volume = 100.0d;
        _prevVolume = _volume;
        _isMuted = false;
        _isMaximized = false;
    }

    private void Mute(object? s)
    {
        if (!_isMuted)
        {
            _isMuted = true;
            _prevVolume = _volume;
            Volume = 0;
            Aggregator.GetEvent<MuteAudio>().Publish(_isMuted);
            return;
        }
        
        _isMuted = false;
        Volume = _prevVolume;
        Aggregator.GetEvent<MuteAudio>().Publish(_isMuted);
    }

    private void Maximize(object? s)
    {
        if (Application.Current.MainWindow is not MetroWindow wnd) return;

        if (!_isMaximized)
        {
            _wndState = wnd.WindowState;
            _wndStyle = wnd.WindowStyle;
            _wndWidth = wnd.Width;
            _wndHeight = wnd.Height;
            
            wnd.WindowState = WindowState.Maximized;
            wnd.WindowStyle = WindowStyle.None;
            wnd.UseNoneWindowStyle = true;

            _isMaximized = true;
            Aggregator.GetEvent<MaximizePlayer>().Publish(_isMaximized);
            return;
        }

        wnd.WindowState = _wndState;
        wnd.WindowStyle = _wndStyle;
        wnd.UseNoneWindowStyle = false;
        wnd.ShowTitleBar = true;
        wnd.Width = _wndWidth;
        wnd.Height = _wndHeight;

        _isMaximized = false;
        Aggregator.GetEvent<MaximizePlayer>().Publish(_isMaximized);
    }
}