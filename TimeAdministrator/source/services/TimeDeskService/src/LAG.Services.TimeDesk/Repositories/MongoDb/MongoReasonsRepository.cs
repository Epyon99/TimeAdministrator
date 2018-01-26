using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPY.Services.LogTiempo.Repositories.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Mgo = EPY.Services.LogTiempo.Repositories.MongoDb.Models;

namespace EPY.Services.LogTiempo.Repositories.MongoDb
{
    public class MongoReasonsRepository : IReasonsRepository
    {
        public const string DateTimeFormatPrecise = "yyyy-MM-ddTHH:mm:ssZ";
        readonly IOptions<MongoDbRepositorySettings> optionsAccessor;

        public MongoReasonsRepository(IOptions<MongoDbRepositorySettings> optionsAccessor)
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
            UserReasonsCollection = SubscriptionDb.GetCollection<Mgo.UserReasons>(Settings.Collection);
        }

        public MongoDbRepositorySettings Settings => optionsAccessor?.Value ?? new MongoDbRepositorySettings();

        public MongoClient Client { get; private set; }

        public IMongoDatabase SubscriptionDb { get; set; }

        public IMongoCollection<Mgo.UserReasons> UserReasonsCollection { get; set; }

        public async Task AddUserReason(UserReasons userReason)
        {
            await UserReasonsCollection
               .InsertOneAsync(
                   new Mgo.UserReasons
                   {
                       UserId = userReason.UserId,
                       Reasons = userReason.Reasons.Select(g => new Mgo.Reasons() { Keyword = g.Keyword, Weight = g.Weight }).ToList(),
                   });
        }

        public async Task<UserReasons> GetAllUserReasons(string userid)
        {
            var userReasons = await UserReasonsCollection.AsQueryable().Where(g => g.UserId == userid)
                  .FirstOrDefaultAsync();
            if (userReasons == null)
            {
                return null;
            }

            var reasons = userReasons.Reasons.OrderByDescending(g => g.Weight);
            var ur = new UserReasons()
            {
                UserId = userid,
                Reasons = reasons.Select(
                    g => new Reasons()
                    {
                        Keyword = g.Keyword,
                        Weight = g.Weight
                    }).ToList(),
            };
            return ur;
        }

        public async Task<UserReasons> GetTopUserReasons(string user)
        {
            var userReasons = await UserReasonsCollection.AsQueryable().Where(g => g.UserId == user)
                  .FirstOrDefaultAsync();
            if (userReasons == null)
            {
                return null;
            }

            var reasons = userReasons.Reasons.OrderByDescending(g => g.Weight).Take(10);
            var ur = new UserReasons()
            {
                UserId = user,
                Reasons = reasons.Select(
                    g => new Reasons()
                    {
                        Keyword = g.Keyword,
                        Weight = g.Weight
                    }).ToList(),
            };
            return ur;
        }

        public async Task UpdateUserReason(UserReasons userReason)
        {
            var ur = new Mgo.UserReasons()
            {
                Reasons = userReason.Reasons.Select(g => new Mgo.Reasons() { Keyword = g.Keyword, Weight = g.Weight }).ToList(),
                UserId = userReason.UserId,
            };

            await UserReasonsCollection.FindOneAndReplaceAsync(g => g.UserId == userReason.UserId, ur);
        }
    }
}
