using System.EventSourcing.Client.Reflection;

namespace EPY.Services.LogTiempo.Projections.Events
{
    public class TimelogUpdatedEvent : TimeLogCreatedEvent
    {
        public string TimeEnd { get; set; }

        public string TimeStartReason { get; set; }

        public string TimeEndReason { get; set; }
    }
}
