using System.EventSourcing.Client.Reflection;

namespace EPY.Services.LogTiempo.Repositories.Models.Events
{
    [Event("timelog", "deleted")]
    public class TimeSpanDeletedEvent
    {
        public string Id { get; set; }
    }
}
