using System.EventSourcing.Client.Reflection;

namespace EPY.Services.TipoLogTiempoService.Services.Models.Events
{
    [Event("types", "created")]
    public class TipoLogTiempoCreateEvent
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public float Factor { get; set; }
    }
}
