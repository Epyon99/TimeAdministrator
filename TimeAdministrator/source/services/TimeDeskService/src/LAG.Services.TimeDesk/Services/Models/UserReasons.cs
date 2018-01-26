using System;
using System.Collections.Generic;
using System.Text;

namespace EPY.Services.LogTiempo.Services.Models
{
    public class UserReasons
    {
        public string UserId { get; set; }

        public List<Reason> Reasons { get; set; }
    }
}
