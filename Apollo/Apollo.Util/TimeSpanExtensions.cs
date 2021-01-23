using System;

namespace Apollo.Util
{
    public static class TimeSpanExtensions
    {

        public static string ToHumanReadable(this TimeSpan timeSpan)
        {
            return $"{timeSpan.Hours:D2}h:{timeSpan.Minutes:D2}m:{timeSpan.Seconds:D2}s:{timeSpan.Milliseconds:D3}ms";
        }
      
    }
}
