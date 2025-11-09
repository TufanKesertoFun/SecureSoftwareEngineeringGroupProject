using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class LogoutModelTests
{
    [Fact]
    public async Task OnPostAsync_SignsOutAndRedirectsToIndex()
    {
        var (model, authService) = CreateModel();

        var result = await model.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
        Assert.True(authService.SignedOut);
    }

    [Fact]
    public async Task OnGetAsync_SignsOutAndRedirectsToIndex()
    {
        var (model, authService) = CreateModel();

        var result = await model.OnGetAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
        Assert.True(authService.SignedOut);
    }

    private static (LogoutModel Model, TestAuthenticationService AuthService) CreateModel()
    {
        var services = new ServiceCollection();
        var authService = new TestAuthenticationService();
        services.AddSingleton<IAuthenticationService>(authService);
        var provider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new RouteData(),
            ActionDescriptor = new PageActionDescriptor()
        };

        var pageContext = new PageContext(actionContext);

        var model = new LogoutModel
        {
            PageContext = pageContext
        };

        return (model, authService);
    }

    private sealed class TestAuthenticationService : IAuthenticationService
    {
        public bool SignedOut { get; private set; }

        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme) =>
            Task.FromResult(AuthenticateResult.NoResult());

        public Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
            Task.CompletedTask;

        public Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
            Task.CompletedTask;

        public Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties)
        {
            SignedOut = false;
            return Task.CompletedTask;
        }

        public Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            SignedOut = true;
            return Task.CompletedTask;
        }
    }
}
