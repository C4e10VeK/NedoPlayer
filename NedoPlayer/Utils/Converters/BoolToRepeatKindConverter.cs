using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace NedoPlayer.Utils.Converters;

[ValueConversion(typeof(bool), typeof(PackIconModernKind))]
public class BoolToRepeatKindConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool v)
            return PackIconMaterialKind.None;

        return v ? PackIconMaterialKind.Repeat : PackIconMaterialKind.None;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}