using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Load configuration (appsettings.json + appsettings.{Env}.json)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// ---- Services ----
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(conn))
    throw new InvalidOperationException("Connection string 'DefaultConnection' is missing or empty.");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(conn));

builder.Services.AddRazorPages(options =>
{
    // Make these pages viewable without signing in
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Signup");
    options.Conventions.AllowAnonymousToPage("/Logout");

    // If later you want most pages private, uncomment this and keep the AllowAnonymous holes above:

});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Login";
        o.AccessDeniedPath = "/Login";
        o.SlidingExpiration = true;
    });

// Add authorization (no global lock-down so Index/Login stay public)
builder.Services.AddAuthorization();

// ---- Pipeline ----
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// IMPORTANT: No manual redirect of "/" — this serves Pages/Index.cshtml by default
app.MapRazorPages();

app.Run();
