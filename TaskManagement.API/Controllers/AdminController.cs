// TaskManagement.API/Controllers/AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.DTOs;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) => _db = db;

        // GET /api/admin/users
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var data = await _db.Users
                .AsNoTracking()
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();

            return Ok(data);
        }

        // POST /api/admin/create-user
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username, Email, and Password are required.");

            var role = (dto.Role ?? "Employee").Trim();
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Role must be 'Admin' or 'Employee'.");

            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists) return Conflict("Email already exists.");

            var user = new TaskManagement.Core.Entities.User
            {
                Username = dto.Username.Trim(),
                Email = dto.Email.Trim(),
                PasswordHash = dto.Password, // TODO: hash
                Role = char.ToUpper(role[0]) + role[1..].ToLower()
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var result = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, result);
        }

        // DELETE /api/admin/users/{id}
        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Optional: prevent deleting self
            var meIdStr = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(meIdStr, out var meId) && meId == id)
                return BadRequest("Cannot delete the currently authenticated admin.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            // Block delete if user still has tasks
            var hasTasks = await _db.Tasks.AnyAsync(t => t.AssignedToUserId == id);
            if (hasTasks) return Conflict("User has assigned tasks. Reassign or delete tasks first.");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
