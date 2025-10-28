using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecureSoftwareGroupProject.Pages
{
    public class LogoutModel : PageModel
    {
        // Shared sign-out routine
        private async Task<IActionResult> SignOutAndRedirectAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostAsync() => await SignOutAndRedirectAsync();

        // Also support GET if you ever need a logout link
        public async Task<IActionResult> OnGetAsync() => await SignOutAndRedirectAsync();
    }
}