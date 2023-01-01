using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using sinves.Models;

namespace sinves.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserService(
            IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _userCollection = mongoDatabase.GetCollection<User>(
                databaseSettings.Value.UserCollection);
        }

        public async Task<User> GetAsync(string getUsername) =>
            await _userCollection.Find(u => u.Username == getUsername).FirstOrDefaultAsync();


        public async Task CreateAsync(User newUser) =>
            await InsertUniqueUser(newUser);
        private async Task InsertUniqueUser(User user)
        {
            var options = new CreateIndexOptions { Unique= true };
            var index = new BsonDocument { { user.Username, user.Username } };
            var model = new CreateIndexModel<User>(index, options);
            try
            {
                await _userCollection.Indexes.CreateOneAsync(model);
                await _userCollection.InsertOneAsync(user);

            } catch(Exception ex)
            {
                throw new Exception("Username exists");
            }
        }
 }
}
