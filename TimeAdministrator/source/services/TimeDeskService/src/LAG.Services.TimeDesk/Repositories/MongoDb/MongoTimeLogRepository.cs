using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EPY.Services.LogTiempo.Repositories.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Mgo = EPY.Services.LogTiempo.Repositories.MongoDb.Models;

namespace EPY.Services.LogTiempo.Repositories.MongoDb
{
    public class MongoTimeLogRepository : ITimeLogRepository
    {
        public const string DateTimeFormatPrecise = "yyyy-MM-ddTHH:mm:ssZ";
        readonly IOptions<MongoDbRepositorySettings> optionsAccessor;

        public MongoTimeLogRepository(IOptions<MongoDbRepositorySettings> optionsAccessor)
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

            SubscriptionDb = Client.GetDatabase(Settings.Db);
            TimeSpanCollection = SubscriptionDb.GetCollection<Mgo.TimeSpan>(Settings.Collection);
        }

        public MongoDbRepositorySettings Settings => optionsAccessor?.Value ?? new MongoDbRepositorySettings();

        public MongoClient Client { get; private set; }

        public IMongoDatabase SubscriptionDb { get; set; }

        public IMongoCollection<Mgo.TimeSpan> TimeSpanCollection { get; set; }

        public async Task CreateTimeLog(TimeLogSpan creation)
        {
            await TimeSpanCollection
                .InsertOneAsync(
                    new Mgo.TimeSpan
                    {
                        Id = creation.Id,
                        TimeStart = DateTime.Parse(creation.TimeStart),
                        UserId = creation.UserId,
                        Edited = false,
                        TimeEnd = creation.TimeEnd != null ? DateTime.Parse(creation.TimeEnd) : default(DateTime?),
                        TipoLogTiempo = creation.TipoLogTiempo,
                        TimeEndReason = string.Empty,
                        TimeStartReason = string.Empty,
                    });
        }

        public async Task DeleteTimeLog(string id)
        {
            await TimeSpanCollection.FindOneAndDeleteAsync(Builders<Mgo.TimeSpan>.Filter.Eq(g => g.Id, id));
        }

        public async Task UpdateTimelog(TimeLogSpan update)
        {
            var result = await TimeSpanCollection
                .ReplaceOneAsync(
                    Builders<Mgo.TimeSpan>.Filter.Eq(g => g.Id, update.Id),
                    new Mgo.TimeSpan()
                    {
                        Id = update.Id,
                        TimeStart = DateTime.Parse(update.TimeStart),
                        UserId = update.UserId,
                        Edited = update.Edited,
                        TimeEndReason = update.TimeEndReason,
                        TimeStartReason = update.TimeStartReason,
                        TimeEnd = update.TimeEnd != null ? DateTime.Parse(update.TimeEnd) : default(DateTime?),
                    });
        }

        public Task<TimeLogSpan> ReadTimeLogById(string logId)
        {
            var result = TimeSpanCollection
                .AsQueryable()
                .Where(g => g.Id == logId)
                .ToList()
                .Select(g => new TimeLogSpan()
                {
                    Id = g.Id,
                    TimeStart = g.TimeStart.ToString(DateTimeFormatPrecise),
                    UserId = g.UserId,
                    Edited = g.Edited,
                    TimeEndReason = g.TimeEndReason,
                    TimeStartReason = g.TimeStartReason,
                    TimeEnd = g.TimeEnd.HasValue ? g.TimeEnd.Value.ToString(DateTimeFormatPrecise) : null,
                });
            return Task.FromResult(result.FirstOrDefault());
        }

        public Task<IEnumerable<TimeLogSpan>> ReadLogsByDateRange(string user, DateTime from, DateTime till)
        {
            from = from.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
            var result = TimeSpanCollection
                .AsQueryable()
                .Where(g => (g.UserId == user) &&
                        ((g.TimeStart >= from && g.TimeEnd < till) || (g.TimeStart >= from && !g.TimeEnd.HasValue)))
                .ToList()
                .OrderBy(g => g.TimeStart)
                .Select(
                    g => new TimeLogSpan
                    {
                        Id = g.Id,
                        TimeStart = g.TimeStart.ToString(DateTimeFormatPrecise),
                        TimeStartReason = g.TimeStartReason,
                        TimeEnd = g.TimeEnd.HasValue ? g.TimeEnd.Value.ToString(DateTimeFormatPrecise) : null,
                        TimeEndReason = g.TimeEndReason,
                        Edited = g.Edited,
                        UserId = g.UserId,
                        TipoLogTiempo = g.TipoLogTiempo,
                    });
            return Task.FromResult(result);
        }

        public async Task<TimeLogSpan> GetLastTimeSpanAsync(string user)
        {
            var select =
                await TimeSpanCollection
                .FindAsync(g => !g.TimeEnd.HasValue && g.UserId == user);

            return select
                .ToList()
                .Select(g => new TimeLogSpan()
                {
                    Id = g.Id,
                    TimeStart = g.TimeStart.ToString(DateTimeFormatPrecise),
                    UserId = g.UserId,
                })
                .FirstOrDefault();
        }
    }
}