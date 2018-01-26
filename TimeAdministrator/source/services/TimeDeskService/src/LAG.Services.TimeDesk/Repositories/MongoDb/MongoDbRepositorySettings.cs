using System.Collections.Generic;

namespace EPY.Services.LogTiempo.Repositories.MongoDb
{
    public class MongoDbRepositorySettings
    {
        public List<MongoDbServer> Servers { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Db { get; set; }

        public string Collection { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool UseSsl { get; set; }
    }
}