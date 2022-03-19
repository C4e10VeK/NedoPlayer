using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NedoPlayer.Utils;

public class FormattedTimeSlider : Slider
{
    private ToolTip? _autoToolTip;

    public string? AutoToolTipFormat { get; set; }

    protected override void OnThumbDragStarted(DragStartedEventArgs e)
    {
        base.OnThumbDragStarted(e);
        FormatAutoToolTip();
    }

    protected override void OnThumbDragDelta(DragDeltaEventArgs e)
    {
        base.OnThumbDragDelta(e);
        FormatAutoToolTip();
    }

    private void FormatAutoToolTip()
    {
        if (!string.IsNullOrWhiteSpace(AutoToolTipFormat))
        {
            if (AutoToolTip != null)
                AutoToolTip.Content = string.Format(AutoToolTipFormat,
                    TimeSpan.FromSeconds(double.Parse((string) AutoToolTip.Content)));
        }
    }

    private ToolTip? AutoToolTip
    {
        get
        {
            if (_autoToolTip == null)
            {
                var field = typeof(Slider).GetField("_autoToolTip", BindingFlags.NonPublic | BindingFlags.Instance);
                Debug.Assert(field != null, nameof(field) + " != null");
                _autoToolTip = field?.GetValue(this) as ToolTip;
            }

            return _autoToolTip;
        }
    }
}