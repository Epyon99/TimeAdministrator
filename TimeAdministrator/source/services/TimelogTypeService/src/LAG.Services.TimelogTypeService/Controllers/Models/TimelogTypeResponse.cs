using System;

namespace EPY.Services.TipoLogTiempoService.Controllers.Models
{
    public class TipoLogTiempoResponse
    {
        public Guid Id { get; set; }

        public string Color { get; set; }

        public float Factor { get; set; }

        public string Name { get; set; }
    }
}
