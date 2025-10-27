using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;

var builder = WebApplication.CreateBuilder(args);

// (Optional: make sure JSONs are loaded explicitly if hosting on IIS)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// 🔎 Read once, guard if missing/empty, and use the variable everywhere
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(conn))
{
    throw new InvalidOperationException(
        $"Connection string 'DefaultConnection' is missing/empty. ENV={builder.Environment.EnvironmentName}");
}

builder.Services.AddRazorPages();

var provider = builder.Configuration.GetValue<string>("Database:Provider") ?? "Sqlite";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(conn);
    }
    else
    {
        var sqliteBuilder = new SqliteConnectionStringBuilder(conn);
        if (!Path.IsPathRooted(sqliteBuilder.DataSource))
        {
            var fullPath = Path.Combine(builder.Environment.ContentRootPath, sqliteBuilder.DataSource);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            sqliteBuilder.DataSource = fullPath;
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(sqliteBuilder.DataSource)!);
        }

        options.UseSqlite(sqliteBuilder.ConnectionString);
    }
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Login";
        o.AccessDeniedPath = "/Login";
        o.SlidingExpiration = true;
    });

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
    await DbInitializer.InitializeAsync(db, logger);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
await app.RunAsync();
