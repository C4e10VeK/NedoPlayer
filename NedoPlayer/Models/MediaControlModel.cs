using System;
using System.Globalization;
using NedoPlayer.Controllers;
using NedoPlayer.Services;

namespace NedoPlayer.Models;

public class MediaControlModel : ModelBase
{
    private readonly IConfigFileService _configFileService;
    private readonly MediaControlController _mediaControlController;
    
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
        set
        {
            Position = TimeSpan.FromSeconds(value);
            _mediaControlController.Seek(Position);
        }
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
            _volume = value < 0 ? 0 : value > 100 ? 100 : value;
            _configFileService.Write("Volume", _volume.ToString(CultureInfo.InvariantCulture));
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
            _configFileService.Write("IsMuted", _isMuted.ToString(CultureInfo.InvariantCulture));
            OnPropertyChanged(nameof(IsMuted));
        }
    }
    
    public MediaControlModel(IConfigFileService configFileService, MediaControlController mediaControlController)
    {
        _configFileService = configFileService;
        _mediaControlController = mediaControlController;
        
        _isPaused = true;
        _position = TimeSpan.FromSeconds(0);
        _totalDuration = TimeSpan.Zero;

        _volume = _configFileService.KeyExists("Volume")
            ? _configFileService.Read<double>("Volume")
            : 100;

        _volumeToFfmpeg = _volume / 100;

        _isMuted = _configFileService.KeyExists("IsMuted") && _configFileService.Read<bool>("IsMuted");
    }
    
    private void NotifySliderDataChanged()
    {
        OnPropertyChanged(nameof(Position));
        OnPropertyChanged(nameof(PositionSeconds));
    }
}