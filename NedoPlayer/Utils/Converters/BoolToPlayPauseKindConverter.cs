using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using MahApps.Metro.IconPacks;

namespace NedoPlayer.Utils.Converters;

[ValueConversion(typeof(bool), typeof(PackIconMaterialKind))]
public class BoolToPlayPauseKindConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool v)
            return PackIconMaterialKind.Pause;

        return v ? PackIconMaterialKind.Play : PackIconMaterialKind.Pause;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

[ValueConversion(typeof(bool), typeof(BitmapImage))]
public class BoolToPlayPauseImgConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool v)
            return new BitmapImage(new Uri(@"pack://application:,,,/Resources/img/pause.png", UriKind.Absolute));

        return v
            ? new BitmapImage(new Uri(@"pack://application:,,,/Resources/img/play.png", UriKind.Absolute))
            : new BitmapImage(new Uri(@"pack://application:,,,/Resources/img/pause.png", UriKind.Absolute));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}