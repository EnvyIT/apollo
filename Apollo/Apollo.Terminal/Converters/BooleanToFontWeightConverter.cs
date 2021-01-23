using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Apollo.Terminal.Converters
{
    public class BooleanToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fontWeight = (FontWeight?) parameter ?? FontWeights.Bold;
            return value != null && (bool) value ? fontWeight : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}