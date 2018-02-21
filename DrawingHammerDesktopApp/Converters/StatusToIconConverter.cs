using DrawingHammerPacketLibrary.Enums;
using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DrawingHammerDesktopApp.Converters
{
    [ValueConversion(typeof(PlayerStatus), typeof(PackIconKind))]
    public class StatusToIconConverter : IValueConverter
    {        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException();
            
            switch ((PlayerStatus) value)
            {
                case PlayerStatus.Guessing:
                    return PackIconKind.Binoculars;                    
                case PlayerStatus.Preparing:
                    return PackIconKind.TooltipImage;                    
                case PlayerStatus.Drawing:
                    return PackIconKind.Pencil;                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }        
    }
}
