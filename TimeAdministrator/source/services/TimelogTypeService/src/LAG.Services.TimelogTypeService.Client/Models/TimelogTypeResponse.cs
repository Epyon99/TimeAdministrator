using System;

namespace EPY.Services.TipoLogTiempoService.Client.Models
{
    public class TipoLogTiempoResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public float Factor { get; set; }
    }
}
