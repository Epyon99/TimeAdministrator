using System;
using System.Collections.Generic;
using System.Text;

namespace EPY.Services.LogTiempo.Services.Models
{
    public class TimelogBookingsRequest
    {
        public DateTimeOffset[] Bookings { get; set; }

        public string TypeId { get; set; }

        public string UserId { get; set; }
    }
}
