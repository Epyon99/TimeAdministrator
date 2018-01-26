using System;

namespace EPY.Services.LogTiempo.Repositories.Models
{
    public class TipoLogTiempo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public float Factor { get; set; }
    }
}
