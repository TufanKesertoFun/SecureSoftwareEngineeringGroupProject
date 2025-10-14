using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using SecureSoftwareGroupProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// EF Core (uses DefaultConnection from appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Login";
        o.AccessDeniedPath = "/Login";
        o.SlidingExpiration = true;
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();                 // <— routing FIRST
app.UseAuthentication();          // then auth
app.UseAuthorization();

app.MapGet("/", ctx =>            // root → /Login
{
    ctx.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.MapRazorPages();
app.Run();
