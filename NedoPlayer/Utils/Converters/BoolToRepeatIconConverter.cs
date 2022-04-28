using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace NedoPlayer.Utils.Converters;

public class BoolToRepeatIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool v)
            return PackIconMaterialKind.Repeat;

        return v ? PackIconMaterialKind.RepeatOff : PackIconMaterialKind.Repeat;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}