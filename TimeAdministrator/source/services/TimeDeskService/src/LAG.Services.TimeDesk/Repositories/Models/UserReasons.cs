using System;
using System.Collections.Generic;
using System.Text;

namespace EPY.Services.LogTiempo.Repositories.Models
{
    public class UserReasons
    {
        public string UserId { get; set; }

        public List<Reasons> Reasons { get; set; }
    }
}
