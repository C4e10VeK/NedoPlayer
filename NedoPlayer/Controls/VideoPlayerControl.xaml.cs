using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NedoPlayer.Controls;

public partial class VideoPlayerControl : UserControl
{
    public TimeSpan PlaybackEndTime
    {
        get => (TimeSpan) GetValue(PlaybackEdnTimeProperty);
        set => SetValue(PlaybackEdnTimeProperty, value);
    }

    public static readonly DependencyProperty PlaybackEdnTimeProperty =
        DependencyProperty.Register("PlaybackEndTime", typeof(TimeSpan), typeof(VideoPlayerControl), new PropertyMetadata(TimeSpan.Zero));

    public TimeSpan Position
    {
        get => (TimeSpan) GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }
    
    public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(TimeSpan), typeof(VideoPlayerControl), new PropertyMetadata(TimeSpan.Zero));
    
    public double Volume
    {
        get => (double) GetValue(VolumeProperty);
        set => SetValue(VolumeProperty, value);
    }
    
    public static readonly DependencyProperty VolumeProperty =
        DependencyProperty.Register("Volume", typeof(double), typeof(VideoPlayerControl), new PropertyMetadata(100d));
    
    public double PositionSecond
    {
        get => (double) GetValue(PositionSecondProperty);
        set => SetValue(PositionSecondProperty, value);
    }
    
    public static readonly DependencyProperty PositionSecondProperty =
        DependencyProperty.Register("PositionSecond", typeof(double), typeof(VideoPlayerControl), new PropertyMetadata(0d));
    
    public bool IsMuted
    {
        get => (bool) GetValue(IsMutedProperty);
        set => SetValue(IsMutedProperty, value);
    }
    
    public static readonly DependencyProperty IsMutedProperty =
        DependencyProperty.Register("IsMuted", typeof(bool), typeof(VideoPlayerControl), new PropertyMetadata(false));
    
    public bool IsPaused
    {
        get => (bool) GetValue(IsPausedProperty);
        set => SetValue(IsPausedProperty, value);
    }
    
    public static readonly DependencyProperty IsPausedProperty =
        DependencyProperty.Register("IsPaused", typeof(bool), typeof(VideoPlayerControl), new PropertyMetadata(false));

    public ICommand PlayPauseCommand
    {
        get => (ICommand) GetValue(PlayPauseCommandProperty);
        set => SetValue(PlayPauseCommandProperty, value);
    }
    
    public static readonly DependencyProperty PlayPauseCommandProperty =
        DependencyProperty.Register("PlayPauseCommand", typeof(ICommand), typeof(VideoPlayerControl), new UIPropertyMetadata(null));
    
    public ICommand MaximizeCommand
    {
        get => (ICommand) GetValue(MaximizeCommandProperty);
        set => SetValue(MaximizeCommandProperty, value);
    }
    
    public static readonly DependencyProperty MaximizeCommandProperty =
        DependencyProperty.Register("MaximizeCommand", typeof(ICommand), typeof(VideoPlayerControl), new UIPropertyMetadata(null));
    
    public ICommand MuteCommand
    {
        get => (ICommand) GetValue(MuteCommandProperty);
        set => SetValue(MuteCommandProperty, value);
    }
    
    public static readonly DependencyProperty MuteCommandProperty =
        DependencyProperty.Register("MuteCommand", typeof(ICommand), typeof(VideoPlayerControl), new UIPropertyMetadata(null));
    
    public ICommand OpenPlaylistCommand
    {
        get => (ICommand) GetValue(OpenPlaylistCommandProperty);
        set => SetValue(OpenPlaylistCommandProperty, value);
    }
    
    public static readonly DependencyProperty OpenPlaylistCommandProperty =
        DependencyProperty.Register("OpenPlaylistCommand", typeof(ICommand), typeof(VideoPlayerControl), new UIPropertyMetadata(null));

    public ICommand NextMediaCommand
    {
        get => (ICommand)GetValue(NextMediaCommandProperty);
        set => SetValue(NextMediaCommandProperty, value);
    }

    public static readonly DependencyProperty NextMediaCommandProperty =
        DependencyProperty.Register("NextMediaCommand", typeof(ICommand), typeof(VideoPlayerControl), new UIPropertyMetadata(null));

    public ICommand PrevMediaCommand
    {
        get => (ICommand)GetValue(PrevMediaCommandProperty);
        set => SetValue(PrevMediaCommandProperty, value);
    }

    public static readonly DependencyProperty PrevMediaCommandProperty =
        DependencyProperty.Register("PrevMediaCommand", typeof(ICommand), typeof(VideoPlayerControl), new UIPropertyMetadata(null));

    public VideoPlayerControl()
    {
        InitializeComponent();
    }
}