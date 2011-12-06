using System;

namespace Serenity.Fixtures
{
    // TODO -- get these into FubuCore already
    public static class DateTimeExtensions
    {
        public static TimeSpan Minutes(this int number)
        {
            return new TimeSpan(0, 0, number, 0);
        }

        public static TimeSpan Hours(this int number)
        {
            return new TimeSpan(0, number, 0, 0);
        }

        public static TimeSpan Days(this int number)
        {
            return new TimeSpan(number, 0, 0, 0);
        }

        public static TimeSpan Seconds(this int number)
        {
            return new TimeSpan(0, 0, number);
        }
    }
}