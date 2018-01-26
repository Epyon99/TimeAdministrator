using System.EventSourcing.Client.Reflection;

namespace EPY.Services.TipoLogTiempoService.Services.Models.Events
{
    [Event("types", "deleted")]
    public class TipoLogTiempoDeleteEvent
    {
        public string Id { get; set; }
    }
}
