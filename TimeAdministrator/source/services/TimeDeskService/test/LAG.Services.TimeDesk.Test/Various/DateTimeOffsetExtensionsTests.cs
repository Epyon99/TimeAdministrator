using System;
using EPY.Services.LogTiempo.Common;
using Xunit;

namespace EPY.LogTiempo.Service.Test.Various
{
    public class DateTimeOffsetExtensionsTests
    {
        [Fact]
        public void GetBeginningOfDay()
        {
            // prepare
            DateTimeOffset date = DateTimeOffset.Now;

            // act
            var date2 = date.GetBeginningOfDay();

            // assert
            Assert.Equal(0, date2.Hour);
            Assert.Equal(0, date2.Minute);
            Assert.Equal(0, date2.Second);
            Assert.Equal(0, date2.Millisecond);
        }

        [Fact]
        public void GetEndOfDay()
        {
            // prepare
            DateTimeOffset date = DateTimeOffset.Now;

            // act
            var date2 = date.GetEndOfDay();

            // assert
            Assert.Equal(23, date2.Hour);
            Assert.Equal(59, date2.Minute);
            Assert.Equal(59, date2.Second);
            Assert.Equal(999, date2.Millisecond);
        }
    }
}
