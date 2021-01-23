using System;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class TimeSpanExtensionTest
    {
        
        [Test]
        public void TimeSpanExtension_ShouldReturnHumanReadableStringFromDate()
        {
            var dateTime = new DateTime(2020, 5, 13, 17, 38, 39, 07);
            var expectedDateTimeString = "17h:38m:39s:007ms";
            var actualDateTimeString = dateTime.TimeOfDay.ToHumanReadable();
            actualDateTimeString.Should().Be(expectedDateTimeString);
        }
    }
}