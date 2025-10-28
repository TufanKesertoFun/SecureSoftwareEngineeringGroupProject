using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;

namespace SecureSoftwareGroupProject.Pages;

[Authorize]
public class ClientsModel : PageModel
{
    public sealed record StatusOption(string Value, string Label);
    public sealed record ClientSummary(
        int Id,
        string Name,
        string Status,
        string? Email,
        string? Phone,
        string? City,
        string? State,
        DateTime LastPaymentDate,
        DateTime RegistrationDate,
        decimal Balance,
        decimal CreditLimit);

    private readonly AppDbContext _db;
    public ClientsModel(AppDbContext db) => _db = db;

    public IReadOnlyList<ClientSummary> Clients { get; private set; } = Array.Empty<ClientSummary>();
    public IReadOnlyList<StatusOption> StatusOptions { get; private set; } = Array.Empty<StatusOption>();

    [BindProperty(SupportsGet = true)] public string? Status { get; set; }
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        // Build status options
        var rawStatuses = await _db.CustomerBalances
            .AsNoTracking()
            .Select(c => c.Status)
            .ToListAsync(ct);

        var normalized = rawStatuses
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var hasUnspecified = rawStatuses.Any(string.IsNullOrWhiteSpace);

        var opts = new List<StatusOption> { new("__all", "All statuses") };
        opts.AddRange(normalized.Select(s => new StatusOption(s, s)));
        if (hasUnspecified) opts.Add(new StatusOption("__unspecified", "Unspecified"));
        StatusOptions = opts;

        // Choose a safe current status
        var defaultStatus = normalized.Count switch
        {
            0 => hasUnspecified ? "__unspecified" : "__all",
            1 => normalized[0],
            _ => "__all"
        };
        var selectedStatus = string.IsNullOrWhiteSpace(Status) ? defaultStatus : Status!;
        if (!StatusOptions.Any(o => string.Equals(o.Value, selectedStatus, StringComparison.OrdinalIgnoreCase)))
            selectedStatus = defaultStatus;
        Status = selectedStatus;

        // Base query
        var q = _db.CustomerBalances.AsNoTracking();

        // Status filter
        if (selectedStatus == "__unspecified")
            q = q.Where(c => c.Status == null || c.Status == "");
        else if (selectedStatus != "__all")
            q = q.Where(c => c.Status != null && c.Status.Trim() == selectedStatus);

        // Search filter
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var term = $"%{Search.Trim()}%";
            q = q.Where(c =>
                EF.Functions.Like(c.CustomerName, term) ||
                (c.Email != null && EF.Functions.Like(c.Email, term)) ||
                (c.City != null && EF.Functions.Like(c.City, term)) ||
                (c.PhoneNumber != null && EF.Functions.Like(c.PhoneNumber, term)));
        }

        // Project
        Clients = await q
            .OrderBy(c => c.CustomerName)
            .Select(c => new ClientSummary(
                c.Id,
                c.CustomerName,
                string.IsNullOrWhiteSpace(c.Status) ? "Unspecified" : c.Status!,
                c.Email,
                c.PhoneNumber,
                c.City,
                c.State,
                c.LastPaymentDate,
                c.RegistrationDate,
                c.Balance,
                c.CreditLimit))
            .ToListAsync(ct);
    }
}
