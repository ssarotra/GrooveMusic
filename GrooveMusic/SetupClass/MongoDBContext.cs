using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System;
using GrooveMusic.SetupClass;

namespace GrooveMusic.Database
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDBSettings> settings)
        {
            try
            {
                Console.WriteLine($"🔹 Connecting to MongoDB: {settings.Value.ConnectionString}");
                var client = new MongoClient(settings.Value.ConnectionString);
                _database = client.GetDatabase(settings.Value.DatabaseName);
                Console.WriteLine("✅ Connected to MongoDB Successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MongoDB Connection Failed: {ex.Message}");
                throw;
            }
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
