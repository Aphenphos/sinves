using Microsoft.Extensions.Options;
using MongoDB.Driver;
using sinves.Models;

namespace sinves.Services
{
    public class BusinessService
    {
        private readonly IMongoCollection<Business> _businessCollection;

        public BusinessService(
            IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _businessCollection = mongoDatabase.GetCollection<Business>(
                databaseSettings.Value.BusinessCollection);
        }

        public async Task<List<Business>> GetAsync() => 
            await _businessCollection.Find(_ => true).ToListAsync();

        public async Task<Business?> GetAsync(string id) =>
            await _businessCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Business newBusiness) =>
            await _businessCollection.InsertOneAsync(newBusiness);

        public async Task UpdateAsync(string id, Business updatedBusiness) =>
            await _businessCollection.ReplaceOneAsync(x => x.Id == id,updatedBusiness);

        public async Task RemoveAsync(string id) =>
            await _businessCollection.DeleteOneAsync(x => x.Id == id);
    }
}
