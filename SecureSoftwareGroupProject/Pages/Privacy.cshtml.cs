using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SecureSoftwareGroupProject.Pages;

[Microsoft.AspNetCore.Authorization.Authorize]
public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;
    public PrivacyModel(ILogger<PrivacyModel> logger) => _logger = logger;
    public void OnGet()
    {
        throw new NotSupportedException("GET requests are not supported on the Customer page.");
    }
}
