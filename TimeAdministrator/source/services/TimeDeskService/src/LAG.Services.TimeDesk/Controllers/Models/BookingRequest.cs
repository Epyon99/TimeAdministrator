using System;
using System.Collections.Generic;
using System.Text;

namespace EPY.Services.LogTiempo.Controllers.Models
{
    public class BookingRequest
    {
        public string[] Dates { get; set; }

        public string Typeid { get; set; }
    }
}
