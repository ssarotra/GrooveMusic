using MongoDB.Driver;
using GrooveMusic.Database;
using System;

namespace GrooveMusic.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDBContext dbContext)
        {
            _users = dbContext.GetCollection<User>("Users");
        }

        public void AddUser(User user)
        {
            try
            {
                Console.WriteLine($"🔹 Attempting to insert user: {user.userName}, ID: {user.userId}");
                _users.InsertOne(user);
                Console.WriteLine($"✅ User inserted successfully in MongoDB: {user.userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MongoDB Insert Failed: {ex.Message}");
            }
        }

        public User GetUserByUsername(string username)
        {
            return _users.Find(u => u.userName == username).FirstOrDefault();
        }

        public void UpdateUser(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.userId, user.userId);
            var update = Builders<User>.Update
                .Set(u => u.RefreshToken, user.RefreshToken)
                .Set(u => u.RefreshTokenExpiry, user.RefreshTokenExpiry);

            _users.UpdateOne(filter, update);
        }

        public User GetUserByRefreshToken(string refreshToken)
        {
            return _users.Find(u => u.RefreshToken == refreshToken).FirstOrDefault();
        }

    }
}
