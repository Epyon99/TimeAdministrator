namespace EPY.Services.LogTiempo.Projections.Events
{
    public class TimeLogCreatedEvent
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Time { get; set; }

        public string TipoLogTiempo { get; set; }
    }
}
