using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GrooveMusic.Database
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string userId { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("userName")]
        public string userName { get; set; }

        [BsonElement("emailId")]
        public List<string> emailId { get; set; } = new List<string>();

        [BsonElement("mobileNumber")]
        public List<MobileNumber> mobileNumber { get; set; } = new List<MobileNumber>();

        [BsonElement("pin")]
        public string pin { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; private set; }

        [BsonElement("recoveryemail")]
        public string? recoveryemail { get; set; }

        [BsonElement("maxScreens")]
        public int MaxScreens { get; set; } = 1;

        [BsonElement("refreshToken")]
        public string RefreshToken { get; set; }

        [BsonElement("refreshTokenExpiry")]
        public DateTime RefreshTokenExpiry { get; set; }

        public void SetPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                PasswordHash = Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return PasswordHash == Convert.ToBase64String(hash);
            }
        }

        public class MobileNumber
        {
            [BsonElement("mobileCode")]
            public string mobileCode { get; set; }

            [BsonElement("mobileNumber")]
            public string mobileNumber { get; set; }
        }
    }
}
