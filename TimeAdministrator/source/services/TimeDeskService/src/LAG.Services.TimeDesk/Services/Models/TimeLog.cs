using System;

namespace EPY.Services.LogTiempo.Repositories.Models
{
    public class TimeLog
    {
        public Guid Id { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}