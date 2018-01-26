using System;

namespace EPY.Services.TipoLogTiempoService.Services.Models
{
    public class TipoLogTiempo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public float Factor { get; set; }
    }
}
