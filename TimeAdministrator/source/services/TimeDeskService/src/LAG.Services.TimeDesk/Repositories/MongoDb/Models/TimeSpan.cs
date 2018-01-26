using System;
using MongoDB.Bson.Serialization.Attributes;

namespace EPY.Services.LogTiempo.Repositories.MongoDb.Models
{
    public class TimeSpan
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("ts")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime TimeStart { get; set; }

        [BsonElement("te")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? TimeEnd { get; set; }

        [BsonElement("edt")]
        public bool Edited { get; set; }

        [BsonElement("tsr")]
        public string TimeStartReason { get; set; }

        [BsonElement("ter")]
        public string TimeEndReason { get; set; }

        [BsonElement("tlt")]
        public string TipoLogTiempo { get; set; }

        [BsonElement("uid")]
        public string UserId { get; set; }
    }
}