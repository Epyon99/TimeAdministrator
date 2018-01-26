using MongoDB.Bson.Serialization.Attributes;

namespace EPY.Services.TipoLogTiempoService.Repositories.MongoDb.Models
{
    public class TipoLogTiempo
    {
        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [BsonElement("factor")]
        public float Factor { get; set; }
    }
}
