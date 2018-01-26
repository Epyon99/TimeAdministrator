using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPY.Services.TipoLogTiempoService.Repositories.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Mgo = EPY.Services.TipoLogTiempoService.Repositories.MongoDb.Models;

namespace EPY.Services.TipoLogTiempoService.Repositories.MongoDb
{
    public class MongoTipoLogTiempoRepository : ITipoLogTiempoRepository
    {
        private readonly IOptions<MongoDbRepositorySettings> optionsAccessor;

        public MongoTipoLogTiempoRepository(IOptions<MongoDbRepositorySettings> optionsAccessor)
        {
            this.optionsAccessor = optionsAccessor;

            var mgoSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(Settings.Host, Settings.Port),
                ConnectTimeout = new TimeSpan(0, 0, 2),
                UseSsl = Settings.UseSsl,
            };

            if (!string.IsNullOrWhiteSpace(Settings.Username) && !string.IsNullOrWhiteSpace(Settings.Password))
            {
                mgoSettings.Credentials = new[]
                {
                    MongoCredential.CreateMongoCRCredential(Settings.Db, Settings.Username, Settings.Password),
                };
            }

            Client = new MongoClient(mgoSettings);

            SubscriptionDb = Client.GetDatabase(Settings.Db);
            TipoLogTiempoCollection = SubscriptionDb.GetCollection<Mgo.TipoLogTiempo>(Settings.Collection);
        }

        public MongoDbRepositorySettings Settings => optionsAccessor?.Value ?? new MongoDbRepositorySettings();

        public MongoClient Client { get; private set; }

        public IMongoDatabase SubscriptionDb { get; set; }

        public IMongoCollection<Mgo.TipoLogTiempo> TipoLogTiempoCollection { get; set; }

        public async Task AddTipoLogTiempo(TipoLogTiempo type)
        {
            await TipoLogTiempoCollection.InsertOneAsync(
                new Mgo.TipoLogTiempo
                {
                    Color = type.Color,
                    Factor = type.Factor,
                    Id = type.Id.ToString(),
                    Name = type.Name,
                });
        }

        public async Task DeleteTipoLogTiempo(string id)
        {
            await TipoLogTiempoCollection.FindOneAndDeleteAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<TipoLogTiempo>> GetAllTimelogsTypes()
        {
            var TipoLogTiempos = TipoLogTiempoCollection.AsQueryable();
            var result = await TipoLogTiempos.ToListAsync();
            var select = result.Select(
                g => new TipoLogTiempo()
                {
                    Color = g.Color,
                    Factor = g.Factor,
                    Id = new Guid(g.Id),
                    Name = g.Name,
                });
            return select;
        }

        public async Task<TipoLogTiempo> GetTimelogById(string id)
        {
            var TipoLogTiempos = await TipoLogTiempoCollection
                .AsQueryable()
                .Where(g => g.Id == id)
                .FirstOrDefaultAsync();
            if (TipoLogTiempos != null)
            {
                return new TipoLogTiempo()
                {
                    Id = new Guid(TipoLogTiempos.Id),
                    Color = TipoLogTiempos.Color,
                    Factor = TipoLogTiempos.Factor,
                    Name = TipoLogTiempos.Name,
                };
            }
            else
            {
                return null;
            }
        }

        public async Task UpdateTipoLogTiempo(TipoLogTiempo type)
        {
            await TipoLogTiempoCollection.ReplaceOneAsync(g => g.Id == type.Id.ToString(), new Mgo.TipoLogTiempo()
            {
                Id = type.Id.ToString(),
                Color = type.Color,
                Factor = type.Factor,
                Name = type.Name,
            });
        }
    }
}