using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace sinves.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String? Id { get; set; }
        [BsonElement("username")]
        public String Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

    }
}
