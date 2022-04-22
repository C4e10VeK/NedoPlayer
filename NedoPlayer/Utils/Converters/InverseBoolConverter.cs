using System;
using System.Globalization;
using System.Windows.Data;

namespace NedoPlayer.Utils.Converters;

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool val)
            throw new InvalidOperationException("Не верный тип. Должен быть bool.");

        return !val;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}