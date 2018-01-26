using System;
using System.Linq;
using System.Threading.Tasks;
using EPY.Services.UserWorkQuota.Repositories.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EPY.Services.UserWorkQuota.Repositories.MongoDb
{
    public class MongoWorkQuotaRepository : IWorkQuotaRepository
    {
        readonly IOptions<MongoDbRepositorySettings> optionsAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoWorkQuotaRepository"/> class.
        /// </summary>
        /// <param name="optionsAccessor">The settings to open a mgo connection</param>
        public MongoWorkQuotaRepository(IOptions<MongoDbRepositorySettings> optionsAccessor)
        {
            this.optionsAccessor = optionsAccessor;

            var mgoSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(Settings.Host, Settings.Port),
                ConnectTimeout = new TimeSpan(0, 0, 2),
                UseSsl = Settings.UseSsl
            };

            if (!string.IsNullOrWhiteSpace(Settings.Username) && !string.IsNullOrWhiteSpace(Settings.Password))
            {
                mgoSettings.Credentials = new[]
                {
                    MongoCredential.CreateMongoCRCredential(Settings.Db, Settings.Username, Settings.Password)
                };
            }

            Client = new MongoClient(mgoSettings);

            QuotasDB = Client.GetDatabase(Settings.Db);
            UserWorkQuotasCollection = QuotasDB.GetCollection<WorkQuota>(Settings.Collection);
        }

        MongoDbRepositorySettings Settings => optionsAccessor?.Value ?? new MongoDbRepositorySettings();

        MongoClient Client { get; set; }

        IMongoDatabase QuotasDB { get; set; }

        IMongoCollection<WorkQuota> UserWorkQuotasCollection { get; set; }

        /// <inheritdoc/>
        public Task CreateWorkQuotaAsync(WorkQuota creation)
        {
            return UserWorkQuotasCollection
                .InsertOneAsync(creation);
        }

        /// <inheritdoc/>
        public Task DeleteWorkQuotaAsync(string id)
        {
            return UserWorkQuotasCollection.FindOneAndDeleteAsync(Builders<WorkQuota>.Filter.Eq("UserId", id));
        }

        /// <inheritdoc/>
        public Task<WorkQuota> ReadWorkQuotaByUserIdAsync(string id)
        {
            var filter = Builders<WorkQuota>.Filter.Eq("UserId", id);
            var result = UserWorkQuotasCollection.Find(filter).FirstOrDefault();
            return Task.FromResult(result);
        }

        /// <inheritdoc/>
        public Task UpdateWorkQuotaAsync(WorkQuota update)
        {
            return UserWorkQuotasCollection.ReplaceOneAsync(Builders<WorkQuota>.Filter.Eq("UserId", update.Id), update);
        }
    }
}
