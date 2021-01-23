using System;
using System.Globalization;
using System.Windows.Data;

namespace Apollo.Terminal.Converters
{
    public class DateTimeToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime dateTime)
            {
                return dateTime.ToShortTimeString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
