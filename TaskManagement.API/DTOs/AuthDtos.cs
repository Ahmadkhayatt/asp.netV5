// TaskManagement.API/DTOs/AuthDtos.cs
namespace TaskManagement.API.DTOs
{
    // ===== Auth =====
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    // ===== Tasks =====
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AssignedToUserId { get; set; }
    }

    public class TaskUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AssignedToUserId { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class TaskStatusDto
    {
        public string Status { get; set; } = "Pending";
    }
}
