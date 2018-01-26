using System.EventSourcing.Client.Reflection;

namespace EPY.Services.TipoLogTiempoService.Services.Models.Events
{
    [Event("types", "update")]
    public class TipoLogTiempoUpdateEvent : TipoLogTiempoCreateEvent
    {
    }
}
