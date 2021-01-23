using System;
using System.Globalization;
using System.Windows.Data;

namespace Apollo.Terminal.Converters
{
    public class BooleanToInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Invert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Invert(value);
        }

        private bool Invert(object value)
        {
            if (value == null || !(value is bool convertedValue))
                throw new InvalidOperationException("The target must be a boolean");

            return !convertedValue;
        }
    }
}