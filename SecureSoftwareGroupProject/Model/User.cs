namespace SecureSoftwareGroupProject.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; } // In production, always store hashed passwords!
    }
}
