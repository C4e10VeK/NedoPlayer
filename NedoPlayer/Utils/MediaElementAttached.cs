using System;
using System.Windows;
using System.Windows.Controls;

namespace NedoPlayer.Utils;

public class MediaElementAttached : DependencyObject
{
    #region VideoPosition Property

    public static DependencyProperty VideoPositionProperty =
        DependencyProperty.RegisterAttached("VideoPosition", typeof(TimeSpan), typeof(MediaElementAttached),
            new PropertyMetadata(TimeSpan.Zero, OnVideoPositionChanged));

    public static TimeSpan GetVideoPosition(DependencyObject d)
    {
        return (TimeSpan) d.GetValue(VideoPositionProperty);
    }
    
    public static void SetVideoPosition(DependencyObject d, TimeSpan value)
    {
        d.SetValue(VideoPositionProperty, value);
    }
    
    private static void OnVideoPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        MediaElement? m = obj as MediaElement;
        
        if (m is null) return;

        m.LoadedBehavior = MediaState.Manual;
        TimeSpan position = (TimeSpan) args.NewValue;
        m.Position = position;
    }

    #endregion

}