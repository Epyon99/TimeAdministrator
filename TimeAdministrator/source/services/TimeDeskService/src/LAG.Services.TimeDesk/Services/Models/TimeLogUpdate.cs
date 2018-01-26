namespace EPY.Services.LogTiempo.Repositories.Models
{
    public class TimeLogUpdate
    {
        public string Id { get; set; }

        public string TimeStart { get; set; }

        public string UserId { get; set; }

        public string TipoLogTiempo { get; set; }

        public string TimeEnd { get; set; }

        public string TimeStartReason { get; set; }

        public string TimeEndReason { get; set; }
    }
}
