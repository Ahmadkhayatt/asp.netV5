using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.DTOs;
using TaskManagement.API.Services;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;
        private readonly IEmailService _email;
        private readonly IConfiguration _cfg;
        private readonly ILogger<AuthController> _log;

        public AuthController(AppDbContext db, JwtService jwt, IEmailService email, IConfiguration cfg, ILogger<AuthController> log)
        {
            _db = db;
            _jwt = jwt;
            _email = email;
            _cfg = cfg;
            _log = log;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized("Invalid credentials");
            if (user.PasswordHash != dto.Password) return Unauthorized("Invalid credentials"); // replace with BCrypt later

            var token = _jwt.GenerateToken(user);
            return Ok(new { token, userId = user.Id, role = user.Role, email = user.Email });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                // Always return 200 to avoid account enumeration
                if (user == null) return Ok();

                var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Replace("+", "").Replace("/", "").Replace("=", "");

                var rec = new Core.Entities.PasswordResetToken
                {
                    UserId = user.Id,
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(1)
                };

                _db.PasswordResetTokens.Add(rec);
                await _db.SaveChangesAsync();

                var baseUrl = _cfg["Email:ResetUrlBase"]?.TrimEnd('/') ?? "http://localhost:3000/reset-password";
                var link = $"{baseUrl}?token={token}";
                await _email.SendAsync(user.Email, "Reset your password",
                    $"<p>Click to reset:</p><p><a href=\"{link}\">{link}</a></p><p>Expires in 1 hour.</p>");

                return Ok();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "ForgotPassword failed");
                // Never 500 on forgot/reset in dev; return Ok to UX
                return Ok();
            }
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var rec = await _db.PasswordResetTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == dto.Token && r.Expiration > DateTime.UtcNow);

            if (rec == null) return BadRequest("Invalid or expired token.");

            rec.User.PasswordHash = dto.NewPassword; // replace with BCrypt later
            _db.PasswordResetTokens.Remove(rec);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me() => Ok(new
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(ClaimTypes.Email),
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}
