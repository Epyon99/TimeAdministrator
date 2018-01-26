using System.Collections.Generic;

namespace EPY.Services.LogTiempo.Hosting.Models
{
    public class KafkaConfiguration
    {
        public IEnumerable<string> BootstrapServers { get; set; }

        public IEnumerable<string> Topics { get; set; }

        public string ConsumerGroup { get; set; }
    }
}
