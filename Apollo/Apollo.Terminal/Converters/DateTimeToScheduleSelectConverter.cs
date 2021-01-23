using System;
using System.Globalization;
using System.Windows.Data;
using Apollo.Terminal.Common;

namespace Apollo.Terminal.Converters
{
    public class DateTimeToScheduleSelectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime dateTime))
            {
                return "";
            }

            var today = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
            return dateTime == today
                ? LocalizationService.GetInstance()["Today"]
                : dateTime.ToString("ddd, dd.MM", LocalizationService.GetInstance().CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}