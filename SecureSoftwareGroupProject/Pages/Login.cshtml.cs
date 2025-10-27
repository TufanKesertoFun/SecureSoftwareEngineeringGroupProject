using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using System.Security.Claims;
namespace SecureSoftwareGroupProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;
        public LoginModel(AppDbContext db) => _db = db;

        [BindProperty] public required string Username { get; set; } = "";
        [BindProperty] public required string Password { get; set; } = "";
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // throw new NotSupportedException("GET requests are not supported on this page.");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == Username && u.Password == Password);

            if (user is null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Customer"); // first step after login
        }
    }
}