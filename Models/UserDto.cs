using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace sinves.Models
{
    public class UserDto
    {
        public String username { get; set; }
        public String password { get; set; }
    }
}
