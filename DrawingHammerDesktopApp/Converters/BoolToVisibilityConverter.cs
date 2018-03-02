using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DrawingHammerDesktopApp.Converters
{
    /// <summary>
    /// Converts bool to visibility
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var visibility = Visibility.Hidden;
            var flag = (bool)value;

            // Set default visibility if parameter is set
            if (parameter is Visibility)
                visibility = (Visibility)parameter;

            return flag ? Visibility.Visible : visibility;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var visibility = (Visibility)value;
            return visibility == Visibility.Visible;
        }
    }
}