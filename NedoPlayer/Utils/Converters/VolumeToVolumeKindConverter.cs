using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace NedoPlayer.Utils.Converters;

public class VolumeToVolumeKindConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) return PackIconMaterialKind.VolumeMute;
        if (values[0] is not double vol || values[1] is not bool muted)
            return PackIconMaterialKind.VolumeMute;

        if (muted)
            return PackIconMaterialKind.VolumeMute;
        
        return vol switch
        {
            > 75 => PackIconMaterialKind.VolumeHigh,
            <= 75 and > 50 => PackIconMaterialKind.VolumeMedium,
            <= 50 and > 25 => PackIconMaterialKind.VolumeMedium,
            <= 25 and > 0 => PackIconMaterialKind.VolumeLow,
            _ => PackIconMaterialKind.VolumeLow
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}