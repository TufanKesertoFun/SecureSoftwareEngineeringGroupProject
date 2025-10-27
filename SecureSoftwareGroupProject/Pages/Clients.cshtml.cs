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

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var rawStatuses = await _db.CustomerBalances
            .AsNoTracking()
            .Select(c => c.Status)
            .Distinct()
            .ToListAsync(cancellationToken);

        var normalizedStatuses = rawStatuses
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
            .Select(s => new StatusOption(s, s))
            .ToList();

        var hasUnspecified = rawStatuses.Any(string.IsNullOrWhiteSpace);

        var options = new List<StatusOption>
        {
            new("__all", "All statuses")
        };
        options.AddRange(normalizedStatuses);
        if (hasUnspecified)
        {
            options.Add(new StatusOption("__unspecified", "Unspecified"));
        }
        StatusOptions = options;

        string defaultStatus;
        if (!normalizedStatuses.Any())
        {
            defaultStatus = hasUnspecified ? "__unspecified" : "__all";
        }
        else if (normalizedStatuses.Count == 1)
        {
            defaultStatus = normalizedStatuses[0].Value;
        }
        else
        {
            defaultStatus = "__all";
        }

        var selectedStatus = string.IsNullOrWhiteSpace(Status) ? defaultStatus : Status;
        if (StatusOptions.All(o => !string.Equals(o.Value, selectedStatus, StringComparison.OrdinalIgnoreCase)))
        {
            selectedStatus = defaultStatus;
        }

        Status = selectedStatus;

        var query = _db.CustomerBalances.AsNoTracking();

        if (string.Equals(selectedStatus, "__all", StringComparison.OrdinalIgnoreCase))
        {
            // no filter
        }
        else if (string.Equals(selectedStatus, "__unspecified", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(c => c.Status == null || c.Status == "");
        }
        else
        {
            query = query.Where(c => c.Status != null &&
                                     c.Status.Trim() == selectedStatus);
        }

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var term = $"%{Search.Trim()}%";
            query = query.Where(c =>
                EF.Functions.Like(c.CustomerName, term) ||
                (c.Email != null && EF.Functions.Like(c.Email, term)) ||
                (c.City != null && EF.Functions.Like(c.City, term)) ||
                (c.PhoneNumber != null && EF.Functions.Like(c.PhoneNumber, term)));
        }

        Clients = await query
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
            .ToListAsync(cancellationToken);
    }
}
