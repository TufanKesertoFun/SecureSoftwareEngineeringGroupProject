using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SecureSoftwareGroupProject.Pages;

[Microsoft.AspNetCore.Authorization.Authorize]
public class CustomerModel : PageModel
{
    private readonly ILogger<CustomerModel> _logger;
    public CustomerModel(ILogger<CustomerModel> logger) => _logger = logger;
    public void OnGet() => _logger.LogDebug("Customer page accessed.");
}
