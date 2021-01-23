using System;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
   public class DateTimeExtensionTest
    {

        private const int FirstDayOfMonth = 1;
        private const int LastDayOfMonthWith28Days = 28;
        private const int LastDayOfMonthWith29Days = 29;
        private const int LastDayOfMonthWith30Days = 30;
        private const int LastDayOfMonthWith31Days = 31;

        private const int OneDayInMinutes = 1440;

        private const int January = 1;
        private const int February = 2;
        private const int March = 3;
        private const int September = 9;
        private const int October = 10;
        private const int December = 12;

        [Test]
        public void GetLastDayOfMonth_ShouldReturn31()
        {
            var normalDateTime = new DateTime(2000, October, 5);
            var firstDayOfMonth= normalDateTime.GetBeginningOfMonth();
            var lastDayOfMonth= normalDateTime.GetEndOfMonth();
            firstDayOfMonth.Day.Should().Be(FirstDayOfMonth);
            firstDayOfMonth.Month.Should().Be(normalDateTime.Month);
            firstDayOfMonth.Year.Should().Be(normalDateTime.Year);
            lastDayOfMonth.Day.Should().Be(LastDayOfMonthWith31Days);
        }

        [Test]
        public void GetLastDayOfMonth_ShouldReturn30()
        {
            var normalDateTime = new DateTime(2000, September, 4);
            var firstDayOfMonth = normalDateTime.GetBeginningOfMonth();
            var lastDayOfMonth = normalDateTime.GetEndOfMonth();
            firstDayOfMonth.Day.Should().Be(FirstDayOfMonth);
            firstDayOfMonth.Month.Should().Be(normalDateTime.Month);
            firstDayOfMonth.Year.Should().Be(normalDateTime.Year);
            lastDayOfMonth.Day.Should().Be(LastDayOfMonthWith30Days);
        }

        [Test]
        public void GetLastDayOfMonth_ShouldReturn28()
        {
            var normalDateTime = new DateTime(2001, February, 27);
            var firstDayOfMonth = normalDateTime.GetBeginningOfMonth();
            var lastDayOfMonth = normalDateTime.GetEndOfMonth();
            DateTime.IsLeapYear(normalDateTime.Year).Should().BeFalse();
            firstDayOfMonth.Day.Should().Be(FirstDayOfMonth);
            firstDayOfMonth.Month.Should().Be(normalDateTime.Month);
            firstDayOfMonth.Year.Should().Be(normalDateTime.Year);
            lastDayOfMonth.Day.Should().Be(LastDayOfMonthWith28Days);
        }

        [Test]
        public void GetLastDayOfMonth_ShouldReturn29()
        {
            var leapYearDateTime = new DateTime(2016, February, 14);
            var firstDayOfMonth = leapYearDateTime.GetBeginningOfMonth();
            var lastDayOfMonth = leapYearDateTime.GetEndOfMonth();
            DateTime.IsLeapYear(leapYearDateTime.Year).Should().BeTrue();
            firstDayOfMonth.Day.Should().Be(FirstDayOfMonth);
            firstDayOfMonth.Month.Should().Be(leapYearDateTime.Month);
            firstDayOfMonth.Year.Should().Be(leapYearDateTime.Year);
            lastDayOfMonth.Day.Should().Be(LastDayOfMonthWith29Days);
        }

        [Test]
        public void DayShouldBeBeforeNextDay()
        {
            var day = new DateTime(2016, February, 14);
            var nextDay = new DateTime(2016, February, 15);
            day.IsBefore(nextDay).Should().BeTrue();
        }

        [Test]
        public void DayShouldBeBeforeNextDay_WhenNewMonth()
        {
            var day = new DateTime(2016, February, LastDayOfMonthWith29Days);
            var nextDay = new DateTime(2016, March, FirstDayOfMonth);
            day.IsBefore(nextDay).Should().BeTrue();
        }

        [Test]
        public void DayShouldBeBeforeNextDay_WhenNewYear()
        {
            var day = new DateTime(2016, December, LastDayOfMonthWith31Days);
            var nextDay = new DateTime(2017, January, FirstDayOfMonth);
            day.IsBefore(nextDay).Should().BeTrue();
        }

        [Test]
        public void WouldExceedDay_OneMinuteBeforeMidnightAddOneMinute_ShouldReturnFalse()
        {
            var day = new DateTime(2016, February, FirstDayOfMonth, 23, 59, 0);
            var exceedsDay = day.WouldExceedDay(1);
            exceedsDay.Should().BeFalse();
        }

        [Test]
        public void WouldExceedDay_OneMinuteBeforeMidnightAddOneMinute_ShouldReturnTrue()
        {
            var day = new DateTime(2016, February, FirstDayOfMonth, 23, 59, 0);
            var exceedsDay = day.WouldExceedDay(2);
            exceedsDay.Should().BeTrue();
        }

        [Test]
        public void WouldExceedDayAndMonth_ShouldReturnFalse()
        {
            var day = new DateTime(2017, February, LastDayOfMonthWith28Days, 0, 0, 0);
            var exceedsDay = day.WouldExceedDay(OneDayInMinutes); 
            exceedsDay.Should().BeFalse();
        }

        [Test]
        public void WouldExceedDayAndMonth_ShouldReturnTrue()
        {
            var day = new DateTime(2017, February, LastDayOfMonthWith28Days, 0, 0, 1);
            var exceedsDay = day.WouldExceedDay(OneDayInMinutes);
            exceedsDay.Should().BeTrue();
        }

        [Test]
        public void IsSameDay_ShouldReturnTrue()
        {
            var day = new DateTime(2020, 12, 3);
            var otherDay = new DateTime(2020, 12, 3);
            var exceedsDay = day.IsSameDay(otherDay);
            exceedsDay.Should().BeTrue();
        }

        [Test]
        public void IsSameDayDifferentDay_ShouldReturnFalse()
        {
            var day = new DateTime(2020, 12, 3);
            var otherDay = new DateTime(2020, 12, 11);
            var exceedsDay = day.IsSameDay(otherDay);
            exceedsDay.Should().BeFalse();
        }

        [Test]
        public void IsSameDayDifferentMonth_ShouldReturnFalse()
        {
            var day = new DateTime(2020, 12, 3);
            var otherDay = new DateTime(2020, 11, 3);
            var exceedsDay = day.IsSameDay(otherDay);
            exceedsDay.Should().BeFalse();
        }

        [Test]
        public void IsSameDayDifferentYear_ShouldReturnFalse()
        {
            var day = new DateTime(2020, 12, 3);
            var otherDay = new DateTime(2002, 12, 3);
            var exceedsDay = day.IsSameDay(otherDay);
            exceedsDay.Should().BeFalse();
        }

    }
}
