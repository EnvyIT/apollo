using System;
using System.Globalization;
using System.Windows.Data;

namespace Apollo.Terminal.Converters.OnScreenKeyboard
{
    public class StringToKeySymbol : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2 || !(values[0] is string symbol) ||
                !(values[1] is bool isShiftActive))
            {
                return "";
            }

            return isShiftActive ? symbol.ToUpper() : symbol.ToLower();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}