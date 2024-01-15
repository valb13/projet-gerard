using MongoDB.Bson.Serialization.Attributes;

namespace Gerardr_Projet_NoSql.Models
{
    [BsonIgnoreExtraElements]
    public class Products
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string Name { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
    }
}
