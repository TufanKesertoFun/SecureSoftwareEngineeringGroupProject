// Pages/SignupDto.cs
using System.ComponentModel.DataAnnotations;

namespace SecureSoftwareGroupProject.Model
{
    public class SignupDto
    {
        [Required, StringLength(64, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
