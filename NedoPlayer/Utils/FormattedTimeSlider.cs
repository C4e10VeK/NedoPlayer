using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using NedoPlayer.Controls;

namespace NedoPlayer.Utils;

public class FormattedTimeSlider : Slider
{
    private ToolTip? _autoToolTip;

    public string? AutoToolTipFormat { get; set; }
    
    public bool IsPaused
    {
        get => (bool) GetValue(IsPausedProperty);
        set => SetValue(IsPausedProperty, value);
    }

    public static readonly DependencyProperty IsPausedProperty =
        DependencyProperty.Register("IsPaused", typeof(bool), typeof(FormattedTimeSlider), new PropertyMetadata(false));
    
    public ICommand PlayCommand
    {
        get => (ICommand) GetValue(PlayCommandProperty);
        set => SetValue(PlayCommandProperty, value);
    }

    public static readonly DependencyProperty PlayCommandProperty =
        DependencyProperty.Register("PlayCommand", typeof(ICommand), typeof(FormattedTimeSlider),
            new UIPropertyMetadata(null));
    
    public ICommand PauseCommand
    {
        get => (ICommand) GetValue(PauseCommandProperty);
        set => SetValue(PauseCommandProperty, value);
    }

    public static readonly DependencyProperty PauseCommandProperty =
        DependencyProperty.Register("PauseCommand", typeof(ICommand), typeof(FormattedTimeSlider),
            new UIPropertyMetadata(null));

    protected override void OnThumbDragStarted(DragStartedEventArgs e)
    {
        base.OnThumbDragStarted(e);
        FormatAutoToolTip();
        
        if (PlayCommand.CanExecute(null))
            PauseCommand.Execute(null);
    }

    protected override void OnThumbDragDelta(DragDeltaEventArgs e)
    {
        base.OnThumbDragDelta(e);
        FormatAutoToolTip();
    }

    protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
    {
        base.OnThumbDragCompleted(e);
        if (!IsPaused && PlayCommand.CanExecute(null))
            PlayCommand.Execute(null);
    }

    private void FormatAutoToolTip()
    {
        if (string.IsNullOrWhiteSpace(AutoToolTipFormat)) return;
        if (AutoToolTip != null)
            AutoToolTip.Content = string.Format(AutoToolTipFormat!,
                TimeSpan.FromSeconds(double.Parse((string) AutoToolTip.Content)));
    }

    private ToolTip? AutoToolTip
    {
        get
        {
            if (_autoToolTip != null) return _autoToolTip;
            var field = typeof(Slider).GetField("_autoToolTip", BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(field != null, nameof(field) + " != null");
            _autoToolTip = field?.GetValue(this) as ToolTip;

            return _autoToolTip;
        }
    }
}