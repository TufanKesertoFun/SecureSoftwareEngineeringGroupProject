using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;

namespace SecureSoftwareGroupProject.Pages
{
    public class IndexModel : PageModel
    {
        public sealed record ProviderHighlight(
            Guid Id,
            string Title,
            string? BusinessName,
            decimal? RatingAverage,
            int? RatingCount,
            decimal? HourlyRate,
            decimal? CalloutFee,
            string? ServiceCategories,
            bool? EmergencyAvailable,
            ReviewHighlight? Review);

        public sealed record ReviewHighlight(
            Guid Id,
            string CustomerName,
            string ReviewText,
            int Rating,
            DateTime CreatedAtUtc);

        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db) => _db = db;

        public IReadOnlyList<ProviderHighlight> TopProviders { get; private set; } = Array.Empty<ProviderHighlight>();
        public IReadOnlyList<ReviewHighlight> TopReviews { get; private set; } = Array.Empty<ReviewHighlight>();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var providers = await _db.ProviderProfiles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            TopReviews = await _db.CustomerReviews
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAtUtc)
                .ThenByDescending(r => r.Rating)
                .Take(4)
                .Select(r => new ReviewHighlight(
                    r.Id,
                    string.IsNullOrWhiteSpace(r.CustomerName) ? "Customer" : r.CustomerName.Trim(),
                    r.ReviewText,
                    r.Rating,
                    r.CreatedAtUtc))
                .ToListAsync(cancellationToken);

            TopProviders = providers
                .OrderByDescending(p => p.RatingAverage ?? 0m)
                .ThenByDescending(p => p.RatingCount ?? 0)
                .ThenByDescending(p => p.CompletedJobsCount ?? 0)
                .ThenBy(p => p.ProfessionalTitle)
                .Take(4)
                .Select((p, index) => new ProviderHighlight(
                    p.Id,
                    string.IsNullOrWhiteSpace(p.ProfessionalTitle) ? "Service Provider" : p.ProfessionalTitle!,
                    p.BusinessName,
                    p.RatingAverage,
                    p.RatingCount,
                    p.HourlyRateAmount,
                    p.CalloutFeeAmount,
                    p.ServiceCategoriesCsv,
                    p.EmergencyAvailableFlag,
                    index < TopReviews.Count ? TopReviews[index] : null))
                .ToList();
        }
    }
}
