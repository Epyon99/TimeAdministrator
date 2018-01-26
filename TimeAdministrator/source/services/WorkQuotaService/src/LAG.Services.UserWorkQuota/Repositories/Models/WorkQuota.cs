using MongoDB.Bson.Serialization.Attributes;

namespace EPY.Services.UserWorkQuota.Repositories.Models
{
    public class WorkQuota
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("quota")]
        public string DailyWorkQuota { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }
    }
}
