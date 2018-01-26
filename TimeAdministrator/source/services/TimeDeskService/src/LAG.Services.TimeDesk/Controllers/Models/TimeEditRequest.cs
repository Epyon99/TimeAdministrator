using Newtonsoft.Json;

namespace EPY.Services.LogTiempo.Controllers.Models
{
    public class TimeEditRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("timeStart")]
        public string TimeStart { get; set; }

        [JsonProperty("timeEnd")]
        public string TimeEnd { get; set; }

        [JsonProperty("timeStartReason")]
        public string TimeStartReason { get; set; }

        [JsonProperty("timeEndReason")]
        public string TimeEndReason { get; set; }
    }
}