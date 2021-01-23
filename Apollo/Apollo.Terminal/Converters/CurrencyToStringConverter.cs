using System;
using System.Globalization;
using System.Windows.Data;

namespace Apollo.Terminal.Converters
{
    public class CurrencyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal price)
            {
                return $"{Decimal.Round(price, 2):#.###,##} \u20AC";
            }

            return $"{0.00:#.###,##} \u20AC";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
