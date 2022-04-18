using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace NedoPlayer.Utils;

public class VolumeToVolumeKindConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) return PackIconModernKind.SoundMute;
        if (values[0] is not double vol || values[1] is not bool muted)
            return PackIconModernKind.SoundMute;

        if (muted)
            return PackIconModernKind.SoundMute;
        
        return vol switch
        {
            > 75 => PackIconModernKind.Sound3,
            <= 75 and > 50 => PackIconModernKind.Sound2,
            <= 50 and > 25 => PackIconModernKind.Sound1,
            <= 25 and > 0 => PackIconModernKind.Sound0,
            _ => PackIconModernKind.Sound0
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}