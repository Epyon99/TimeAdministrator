using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace EPY.Services.LogTiempo.Repositories.MongoDb.Models
{
    public class Reasons
    {
        [BsonElement]
        public string Keyword { get; set; }

        [BsonElement]
        public int Weight { get; set; }
    }
}
