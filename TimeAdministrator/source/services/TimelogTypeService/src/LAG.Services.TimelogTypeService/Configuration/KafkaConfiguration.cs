using System.Collections.Generic;

namespace EPY.Services.TipoLogTiempoService.Configuration
{
    public class KafkaConfiguration
    {
        public IEnumerable<string> BootstrapServers { get; set; }

        public IEnumerable<string> Topics { get; set; }

        public string ConsumerGroup { get; set; }
    }
}
