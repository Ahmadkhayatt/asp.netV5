namespace TaskManagement.Core.Entities
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }

        // navigation property
        public User User { get; set; } = null!;
    }
}
