using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SecureSoftwareGroupProject.Pages
{
   
    public class SignupModel : PageModel
    {
        private readonly AppDbContext _db;
        public SignupModel(AppDbContext db) => _db = db;

        [BindProperty, Required, StringLength(64, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [BindProperty, Required, StringLength(72, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,72}$",
            ErrorMessage = "Password must include upper, lower, number, and symbol.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [BindProperty, Required, DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var exists = await _db.Users.AnyAsync(u => u.Username == Username);
            if (exists)
            {
                ModelState.AddModelError(nameof(Username), "This username is already taken.");
                return Page();
            }

            // hash the password
            string hash = BCrypt.Net.BCrypt.HashPassword(Password, workFactor: 12);

            // store hash in both columns to satisfy NOT NULL on Password
            var user = new User
            {
                Username = Username.Trim(),
                PasswordHash = hash,
                Password = hash,
                Role = "User"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // sign-in
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToPage("/Customer");
        }
    }
}
