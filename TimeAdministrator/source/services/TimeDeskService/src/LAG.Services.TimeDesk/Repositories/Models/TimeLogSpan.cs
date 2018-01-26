namespace EPY.Services.LogTiempo.Repositories.Models
{
    public class TimeLogSpan
    {
        public string Id { get; set; }

        public string TimeStart { get; set; }

        public string TimeEnd { get; set; }

        public string UserId { get; set; }

        public string TipoLogTiempo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value that indicates if the user manually modify the dates.
        /// use only for Update operations.
        /// </summary>
        public bool Edited { get; set; }

        public string TimeStartReason { get; set; }

        public string TimeEndReason { get; set; }
    }
}