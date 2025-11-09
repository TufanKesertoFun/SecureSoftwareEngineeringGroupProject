using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Models;
using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class LoginModelTests
{
    [Fact]
    public async Task OnPostAsync_InvalidModelState_ReturnsPage()
    {
        await using var context = CreateContext();
        var model = CreateModel(context, out var authService);
        model.ModelState.AddModelError("Username", "Required");
        model.Username = "";
        model.Password = "";

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.False(authService.SignedIn);
    }

    [Fact]
    public async Task OnPostAsync_InvalidCredentials_ReturnsPageWithError()
    {
        var user = CreateUser("alice", BCrypt.Net.BCrypt.HashPassword("Correct"));
        await using var context = CreateContext(user);
        var model = CreateModel(context, out var authService);
        model.Username = "alice";
        model.Password = "WrongPassword";

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("Invalid username or password.", model.ErrorMessage);
        Assert.False(authService.SignedIn);
    }

    [Fact]
    public async Task OnPostAsync_ValidCredentials_SignsInAndRedirects()
    {
        var hashed = BCrypt.Net.BCrypt.HashPassword("Correct");
        var user = CreateUser("alice", hashed, "Admin");
        await using var context = CreateContext(user);
        var model = CreateModel(context, out var authService);
        model.Username = "alice";
        model.Password = "Correct";

        var result = await model.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Clients", redirect.PageName);
        Assert.True(authService.SignedIn);
        Assert.Equal("alice", authService.Principal?.Identity?.Name);
        Assert.Contains(authService.Principal?.Claims ?? Array.Empty<Claim>(), c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    private static LoginModel CreateModel(AppDbContext context, out TestAuthenticationService authService)
    {
        var services = new ServiceCollection();
        authService = new TestAuthenticationService();
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

        var model = new LoginModel(context);
        model.PageContext = pageContext;

        return model;
    }

    private static AppDbContext CreateContext(params User[] users)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        if (users.Length > 0)
        {
            context.Users.AddRange(users);
            context.SaveChanges();
        }
        return context;
    }

    private static User CreateUser(string username, string passwordHash, string role = "User") =>
        new()
        {
            Username = username,
            Password = "legacy",
            PasswordHash = passwordHash,
            Role = role
        };

    private sealed class TestAuthenticationService : IAuthenticationService
    {
        public bool SignedIn { get; private set; }
        public ClaimsPrincipal? Principal { get; private set; }

        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme) =>
            Task.FromResult(AuthenticateResult.NoResult());

        public Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
            Task.CompletedTask;

        public Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
            Task.CompletedTask;

        public Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties)
        {
            SignedIn = true;
            Principal = principal;
            return Task.CompletedTask;
        }

        public Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            SignedIn = false;
            Principal = null;
            return Task.CompletedTask;
        }
    }
}
