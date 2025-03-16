using MongoDB.Driver;
using GrooveMusic.Database;
using GrooveMusic.SetupClass;
using System.Collections.Generic;

namespace GrooveMusic.Repositories
{
    public class SessionRepository
    {
        private readonly IMongoCollection<Session> _sessions;

        public SessionRepository(MongoDBContext dbContext)
        {
            _sessions = dbContext.GetCollection<Session>("Sessions");
        }

        public void AddSession(Session session) => _sessions.InsertOne(session);

        public List<Session> GetActiveSessions(string userId)
        {
            return _sessions.Find(s => s.UserId == userId && s.IsActive).ToList();
        }

        // ✅ Fix: Now allows removing a session by both userId and sessionId
        public void RemoveSession(string userId, string sessionId)
        {
            var filter = Builders<Session>.Filter.And(
                Builders<Session>.Filter.Eq(s => s.UserId, userId),
                Builders<Session>.Filter.Eq(s => s.SessionId, sessionId)
            );
            _sessions.DeleteOne(filter);
        }

        public void RemoveAllSessionsForUser(string userId)
        {
            _sessions.DeleteMany(s => s.UserId == userId);
        }
    }
}
