using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace NedoPlayer.Utils.Converters;

[ValueConversion(typeof(int), typeof(PackIconModernKind))]
public class IntToRepeatKindConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int v)
            return PackIconModernKind.None;

        return v > 0 ? PackIconModernKind.Repeat : PackIconModernKind.None;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}