using System.Collections.Generic;

namespace EPY.Services.UserWorkQuota.Repositories.MongoDb
{
    /// <summary>
    /// Mongodb Settings
    /// </summary>
    public class MongoDbRepositorySettings
    {
        /// <summary>
        /// Gets or sets the servers in the cluster
        /// </summary>
        public List<MongoDbServer> Servers { get; set; }

        /// <summary>
        /// Gets or sets the hostname
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the database to use
        /// </summary>
        public string Db { get; set; }

        /// <summary>
        /// Gets or sets the collections name
        /// </summary>
        public string Collection { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use ssl or not
        /// </summary>
        public bool UseSsl { get; set; }
    }
}
