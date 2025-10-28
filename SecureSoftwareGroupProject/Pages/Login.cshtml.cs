using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SecureSoftwareGroupProject.Pages
{
   
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;
        public LoginModel(AppDbContext db) => _db = db;

        [BindProperty, Required] public string Username { get; set; } = "";
        [BindProperty, Required, DataType(DataType.Password)] public string Password { get; set; } = "";
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            //This has purposfully been left black to allow for any future changes
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _db.Users.AsNoTracking()
                                      .FirstOrDefaultAsync(u => u.Username == Username);

            var stored = user?.PasswordHash;                  // use hash column
            var ok = !string.IsNullOrWhiteSpace(stored) &&
                     BCrypt.Net.BCrypt.Verify(Password, stored);

            if (!ok)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user!.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToPage("/Clients");
        }
    }
}
