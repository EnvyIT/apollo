using System;
using System.Globalization;
using System.Windows.Data;

namespace Apollo.Terminal.Converters
{
    public class StringToShortenStringConverter : IMultiValueConverter
    {
    
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3)
            {
                if (values[0] is string input && values[1] is int maxCharacters && values[2] is string shortenText)
                {
                    return input?.Length > maxCharacters ? $"{input.Substring(0, maxCharacters)} {shortenText}" : input;
                }
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }
    }
}
