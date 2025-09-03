namespace TaskManagement.Core.Entities
{
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Done
    }

    public class TaskItem
    {
        public int Id { get; set; }  // Primary Key
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int AssignedToUserId { get; set; }

        // Navigation property
        public User? AssignedToUser { get; set; }
    }
}
     