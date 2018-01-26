using System;
using System.Collections.Generic;
using System.Text;

namespace EPY.Services.LogTiempo.Controllers.Models
{
    public class TipoLogTiempoResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public float Factor { get; set; }
    }
}
