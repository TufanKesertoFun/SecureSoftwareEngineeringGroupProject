using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;

namespace SecureSoftwareGroupProject.Pages;

[Authorize]
public class CustomerModel : PageModel
{
    public sealed record RowVm(
        int Id,
        string CustomerName,
        string AccountNumber,
        decimal Balance,
        string Currency,
        DateTime LastPaymentDate,
        decimal CreditLimit,
        string? Status,
        string? PhoneNumber,
        string? Email,
        string? Address,
        string? City,
        string? State,
        string? PostalCode,
        DateTime RegistrationDate);

    private readonly AppDbContext _db;
    public CustomerModel(AppDbContext db) => _db = db;

    public List<RowVm> Rows { get; private set; } = new();
    public List<string> AllCurrencies { get; private set; } = new();

    // Filters (GET)
    [BindProperty(SupportsGet = true)] public string Status { get; set; } = "__all";
    [BindProperty(SupportsGet = true)] public string Currency { get; set; } = "__all";
    [BindProperty(SupportsGet = true)] public decimal? MinBalance { get; set; }
    [BindProperty(SupportsGet = true)] public string? Query { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        // Currencies for filter dropdown
        AllCurrencies = await _db.CustomerBalances
            .AsNoTracking()
            .Where(c => c.Currency != null && c.Currency != "")
            .Select(c => c.Currency!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);

        // Base query
        var q = _db.CustomerBalances.AsNoTracking();

        // Status filter
        if (!string.Equals(Status, "__all", StringComparison.OrdinalIgnoreCase))
            q = q.Where(c => (c.Status ?? "").Trim() == Status);

        // Currency filter
        if (!string.Equals(Currency, "__all", StringComparison.OrdinalIgnoreCase))
            q = q.Where(c => c.Currency == Currency);

        // Min balance
        if (MinBalance.HasValue)
            q = q.Where(c => c.Balance >= MinBalance.Value);

        // Text search
        if (!string.IsNullOrWhiteSpace(Query))
        {
            var term = $"%{Query.Trim()}%";
            q = q.Where(c =>
                EF.Functions.Like(c.CustomerName, term) ||
                (c.Email != null && EF.Functions.Like(c.Email, term)) ||
                (c.City != null && EF.Functions.Like(c.City, term)) ||
                (c.PhoneNumber != null && EF.Functions.Like(c.PhoneNumber, term)) ||
                (c.AccountNumber != null && EF.Functions.Like(c.AccountNumber, term)));
        }

        // Project to lightweight VM
        Rows = await q
            .OrderByDescending(c => c.LastPaymentDate) // useful default
            .Select(c => new RowVm(
                c.Id,
                c.CustomerName,
                c.AccountNumber,
                c.Balance,
                c.Currency ?? "",
                c.LastPaymentDate,
                c.CreditLimit,
                c.Status,
                c.PhoneNumber,
                c.Email,
                c.Address,
                c.City,
                c.State,
                c.PostalCode,
                c.RegistrationDate))
            .ToListAsync(ct);
    }
}
