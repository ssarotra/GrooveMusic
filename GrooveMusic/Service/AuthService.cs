using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using GrooveMusic.Database;
using MongoDB.Driver;
using GrooveMusic.Repositories;
using System.Security.Cryptography;

namespace GrooveMusic.Service
{
    public class AuthService
    {
        private readonly UserRepository _userRepo;
        private readonly SessionRepository _sessionRepo;
        private readonly string jwtSecret = "SuperSecureJWTSecretKeyForAuthentication@123456!";
        private readonly int refreshTokenExpiryDays = 7; // Refresh token validity

        public AuthService(UserRepository userRepo, SessionRepository sessionRepo)
        {
            _userRepo = userRepo;
            _sessionRepo = sessionRepo;
        }

        public User Register(RegisterRequest request)
        {
            if (_userRepo.GetUserByUsername(request.userName) != null)
                throw new Exception("Username already taken");

            var user = new User
            {
                userName = request.userName,
                emailId = request.emailIds,
                pin = request.pin,
                mobileNumber = request.mobileNumbers,
                recoveryemail = request.recoveryemail,
                MaxScreens = request.MaxScreens > 0 ? request.MaxScreens : 1,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpiryDays) // 7 days validity
            };

            user.SetPassword(request.password);
            _userRepo.AddUser(user); // Save user in MongoDB

            Console.WriteLine($"✅ User Registered: {user.userName} (ID: {user.userId})");
            return user;
        }

        public (string accessToken, string refreshToken) Login(string userName, string password)
        {
            var user = _userRepo.GetUserByUsername(userName);
            if (user == null || !user.VerifyPassword(password))
                throw new Exception("Invalid credentials");

            var activeSessions = _sessionRepo.GetActiveSessions(user.userId);
            if (activeSessions.Count >= user.MaxScreens)
                throw new Exception($"Maximum screens limit reached ({user.MaxScreens}). Please log out from another device.");

            string accessToken = GenerateJwtToken(user);
            string refreshToken = GenerateRefreshToken();

            // ✅ Store refresh token in MongoDB
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);
            _userRepo.UpdateUser(user); // 🔹 Save refresh token

            // ✅ Store session in MongoDB
            var session = new Session
            {
                UserId = user.userId,
                JwtToken = accessToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsActive = true
            };
            _sessionRepo.AddSession(session); // 🔹 Save session in MongoDB

            Console.WriteLine($"✅ Session Created for User: {user.userName} (Session ID: {session.SessionId})");

            return (accessToken, refreshToken);
        }

        public string RefreshAccessToken(string refreshToken)
        {
            var user = _userRepo.GetUserByRefreshToken(refreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token.");

            string newAccessToken = GenerateJwtToken(user);

            Console.WriteLine($"✅ New Access Token Generated for User: {user.userName}");

            return newAccessToken;
        }

        public void Logout(string userId, string sessionId)
        {
            _sessionRepo.RemoveSession(userId, sessionId);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.userId),
                    new Claim("userName", user.userName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public string TestMongoDBConnection()
        {
            string connectionString = "mongodb+srv://GrooveMusic:Y7pfz9jzsyWeQ7vB@groovemusic.0cjf0.mongodb.net/?authMechanism=SCRAM-SHA-256";
            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("GrooveMusic");
                return "✅ Connected to MongoDB Successfully!";
            }
            catch (Exception ex)
            {
                return "❌ MongoDB Connection Error: " + ex.Message;
            }
        }
    }
}
