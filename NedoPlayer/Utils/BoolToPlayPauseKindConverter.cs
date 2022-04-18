using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace NedoPlayer.Utils;

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