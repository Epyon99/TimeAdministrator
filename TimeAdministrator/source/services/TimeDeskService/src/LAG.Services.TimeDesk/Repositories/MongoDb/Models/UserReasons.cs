using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace EPY.Services.LogTiempo.Repositories.MongoDb.Models
{
    public class UserReasons
    {
        [BsonId]
        public string UserId { get; set; }

        [BsonElement]
        public List<Reasons> Reasons { get; set; }
    }
}
