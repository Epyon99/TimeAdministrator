namespace EPY.Services.TipoLogTiempoService.Projections.Events
{
    public class TipoLogTiempoCreateEvent
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public float Factor { get; set; }
    }
}
