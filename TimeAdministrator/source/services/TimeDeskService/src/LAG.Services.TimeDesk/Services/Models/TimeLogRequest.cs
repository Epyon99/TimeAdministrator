using System;

namespace EPY.Services.LogTiempo.Repositories.Models
{
    public class TimeLogRequest
    {
        public string UserId { get; set; }

        public string LogType { get; set; }

        public DateTime Date { get; set; }

        public WorkQuota UserWorkQuota { get; set; }
    }
}
