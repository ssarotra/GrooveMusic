using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GrooveMusic.Database
{
    public class Session
    {
        [BsonId] // ✅ Marks this as the MongoDB primary key
        [BsonRepresentation(BsonType.ObjectId)]
        public string SessionId { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("userId")] // ✅ Links session to a specific user
        public string UserId { get; set; }

        [BsonElement("jwtToken")] // ✅ Stores the JWT access token
        public string JwtToken { get; set; }

        [BsonElement("createdAt")] // ✅ Timestamp of when the session started
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("expiresAt")] // ✅ Optional: Track when session should expire
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(1);

        [BsonElement("isActive")] // ✅ Used to check if the session is still valid
        public bool IsActive { get; set; } = true;
    }
}
