// TaskManagement.API/DTOs/UserDtos.cs
namespace TaskManagement.API.DTOs
{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // TODO: hash later
        public string Role { get; set; } = "Employee";   // "Admin" or "Employee"
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee";
    }
}
