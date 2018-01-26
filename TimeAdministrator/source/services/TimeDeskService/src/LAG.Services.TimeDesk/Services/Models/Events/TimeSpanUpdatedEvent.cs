using System.EventSourcing.Client.Reflection;

namespace EPY.Services.LogTiempo.Repositories.Models.Events
{
    [Event("timelog", "updated")]
    public class TimeSpanUpdatedEvent
    {
        public string Id { get; set; }

        public string Time { get; set; }

        public string UserId { get; set; }

        public string TipoLogTiempo { get; set; }

        public string TimeEnd { get; set; }

        public string TimeStartReason { get; set; }

        public string TimeEndReason { get; set; }
    }
}
