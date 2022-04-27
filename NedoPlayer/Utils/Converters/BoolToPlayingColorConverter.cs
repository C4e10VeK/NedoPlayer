using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ControlzEx.Theming;

namespace NedoPlayer.Utils.Converters;

[ValueConversion(typeof(bool), typeof(Brush))]
public class BoolToPlayingColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool v)
            return (Brush)ThemeManager.Current.DetectTheme(Application.Current)!.Resources["MahApps.Brushes.IdealForeground"];

        return v ? Brushes.Chartreuse : (Brush)ThemeManager.Current.DetectTheme(Application.Current)!.Resources["MahApps.Brushes.IdealForeground"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}