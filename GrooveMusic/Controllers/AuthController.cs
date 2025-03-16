using Microsoft.AspNetCore.Mvc;
using GrooveMusic.Database;
using GrooveMusic.Service; // ✅ Corrected namespace for AuthService
using GrooveMusic.Models;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = _authService.Register(request);
            return Ok(new { Message = "User registered successfully", UserId = user.userId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            // ✅ Fix: Return both access and refresh tokens
            var (accessToken, refreshToken) = _authService.Login(request.UserName, request.Password);
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout([FromBody] LogoutRequest request)
    {
        try
        {
            _authService.Logout(request.UserId, request.SessionId);
            return Ok(new { Message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            string newAccessToken = _authService.RefreshAccessToken(request.RefreshToken);
            return Ok(new { Token = newAccessToken });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("test-connection")]
    public IActionResult TestMongoDBConnection()
    {
        try
        {
            string message = _authService.TestMongoDBConnection();
            return Ok(new { Message = message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
