using System;

namespace EPY.Services.LogTiempo.Common
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset GetBeginningOfDay(this DateTimeOffset value)
        {
            return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, 0, value.Offset);
        }

        public static DateTimeOffset GetEndOfDay(this DateTimeOffset value)
        {
            return new DateTimeOffset(value.Year, value.Month, value.Day, 23, 59, 59, 999, value.Offset);
        }
    }
}
