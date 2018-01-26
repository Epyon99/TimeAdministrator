using System;

namespace EPY.Services.LogTiempo.Repositories.Models
{
    public class LogTimeSpan
    {
        public string Id { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public string Type { get; set; }

        public string StartReason { get; set; }

        public string EndReason { get; set; }

        public bool Edited { get; set; }
    }
}
