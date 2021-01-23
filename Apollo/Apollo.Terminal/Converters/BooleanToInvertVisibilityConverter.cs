using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Apollo.Terminal.Converters
{
    public class BooleanToInvertVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invertConverter = new BooleanToInvertConverter();
            var visibilityConverter = new BooleanToVisibilityConverter();

            return visibilityConverter
                .Convert(invertConverter
                        .Convert(value, targetType, parameter, culture),
                    targetType, parameter, culture
                );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invertConverter = new BooleanToInvertConverter();
            var visibilityConverter = new BooleanToVisibilityConverter();

            return invertConverter
                .Convert(visibilityConverter
                        .Convert(value, targetType, parameter, culture),
                    targetType, parameter, culture
                );
        }
    }
}