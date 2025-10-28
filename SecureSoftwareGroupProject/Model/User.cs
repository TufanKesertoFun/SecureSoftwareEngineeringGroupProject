using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureSoftwareGroupProject.Models
{
    [Table("Users")]
    public class User
    {
        [Key] public int Id { get; set; }

        [Required, StringLength(64)]
        public string Username { get; set; } = string.Empty;

        // Legacy (nullable). Keep for old rows; do not write new plaintext here.
        public string? Password { get; set; }

        // Primary credential going forward.
        [StringLength(200)]
        public string? PasswordHash { get; set; }

        public string Role { get; set; } = "User";

    }
}
