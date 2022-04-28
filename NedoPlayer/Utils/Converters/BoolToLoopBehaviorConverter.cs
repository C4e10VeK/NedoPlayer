using System;
using System.Globalization;
using System.Windows.Data;
using Unosquare.FFME.Common;

namespace NedoPlayer.Utils.Converters;

[ValueConversion(typeof(bool), typeof(MediaPlaybackState))]
public class BoolToLoopBehaviorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool v)
            return MediaPlaybackState.Pause;

        return v ? MediaPlaybackState.Play : MediaPlaybackState.Pause;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}