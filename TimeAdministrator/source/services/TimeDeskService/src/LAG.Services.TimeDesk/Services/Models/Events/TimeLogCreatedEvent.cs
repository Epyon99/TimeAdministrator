using System.EventSourcing.Client.Reflection;

namespace EPY.Services.LogTiempo.Repositories.Models.Events
{
    [Event("timelog", "created")]
    public class TimeLogCreatedEvent
    {
        public string Id { get; set; }

        public string Time { get; set; }

        public string UserId { get; set; }

        public string TipoLogTiempo { get; set; }
    }
}
