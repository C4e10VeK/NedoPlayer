using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using NedoPlayer.Models;

namespace NedoPlayer.Utils.Converters;

[ValueConversion(typeof(IEnumerable<object>), typeof(string))]
public class ItemsToDurationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IEnumerable<object> medias) return "00:00:00";
            
        var res = medias.Aggregate(TimeSpan.Zero, (current, m) => current + ((MediaInfo) m).Duration.GetValueOrDefault());

        return res.ToString("hh\\:mm\\:ss");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

[ValueConversion(typeof(IEnumerable<object>), typeof(string))]
public class ItemsToFolderNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IEnumerable<object> medias) return "";

        MediaInfo? res = medias.First() as MediaInfo;

        return res is null ? "" : res.Path;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}