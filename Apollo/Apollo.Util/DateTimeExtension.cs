using System;

namespace Apollo.Util
{
    public static class DateTimeExtension
    {
        private const int OneDay = 1;
        private const int OneMonth = 1;

        public static DateTime GetBeginningOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, OneDay);
        }

        public static DateTime GetEndOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

        public static bool IsBefore(this DateTime dateTime, DateTime other)
        {
            return ((dateTime.Day < other.Day && dateTime.Month == other.Month && dateTime.Year == other.Year) ||
                    (dateTime.Day > other.Day && dateTime.Month < other.Month && dateTime.Year == other.Year) ||
                    (dateTime.Day > other.Day && dateTime.Month > other.Month && dateTime.Year < other.Year));
        }

        public static bool WouldExceedDay(this DateTime dateTime, long minutes)
        {
            var beginningOfDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
            return dateTime.AddMinutes(minutes) > beginningOfDay.AddDays(1);
        }

        public static bool IsSameDay(this DateTime dateTime, DateTime other)
        {
            return (dateTime.Day == other.Day && dateTime.Month == other.Month && dateTime.Year == other.Year);
        }

        public static bool IsWeekend(this DateTime dateTime)
        {
            return dateTime.DayOfWeek == DayOfWeek.Friday ||
                   dateTime.DayOfWeek == DayOfWeek.Saturday ||
                   dateTime.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}