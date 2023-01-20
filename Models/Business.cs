using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Reflection.Metadata;

namespace sinves.Models
{
    public class Business
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String? Id { get; set; }

        [BsonElement("Name")]
        public String name { get; set; }
        public String[] categories { get; set; }
        public String[]? links { get; set; }
        public String[]? imageLinks { get; set; } = null!;
        public String hours { get; set; } = null!;
    }
}
