namespace SecureSoftwareGroupProject.Models
{
    public class User
    {
        public int Id { get; set; }

        public required string Username { get; set; }

        public required string Password { get; set; } // In production, always store hashed passwords!
    }
}
