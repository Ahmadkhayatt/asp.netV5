namespace TaskManagement.Core.Entities
{
    public class User
    {
        public int Id { get; set; }  // Primary Key
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee"; // Default role is Employee

        // Navigation property (1 user can have many tasks)
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
