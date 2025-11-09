using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Model;
using SecureSoftwareGroupProject.Models;
using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class IndexModelTests
{
    [Fact]
    public async Task OnGetAsync_LoadsTopProvidersAndReviews()
    {
        var now = DateTime.UtcNow;
        await using var context = CreateContext(
            providers: new[]
            {
                CreateProvider("Alpha", 4.9m, 25, 200),
                CreateProvider("Bravo", 4.5m, 50, 150),
                CreateProvider("Charlie", 4.9m, 10, 50),
                CreateProvider("Delta", 4.2m, 5, 20),
                CreateProvider("Echo", 3.5m, 30, 300)
            },
            reviews: new[]
            {
                CreateReview("Alice", "Great!", 5, now),
                CreateReview("Bob", "Solid work", 4, now.AddMinutes(-10)),
                CreateReview("   ", "Anonymous", 5, now.AddMinutes(-5)),
                CreateReview("Cara", "Average", 3, now.AddMinutes(-30)),
                CreateReview("Dan", "Old", 5, now.AddHours(-1))
            });

        var model = new IndexModel(context);

        await model.OnGetAsync(CancellationToken.None);

        Assert.Equal(4, model.TopReviews.Count);
        Assert.Equal(new[] { "Alice", "Customer", "Bob", "Cara" }, model.TopReviews.Select(r => r.CustomerName));
        Assert.Equal(4, model.TopProviders.Count);
        Assert.Equal(new[] { "Alpha", "Charlie", "Bravo", "Delta" }, model.TopProviders.Select(p => p.Title));
        Assert.Equal("Great!", model.TopProviders[0].Review!.ReviewText);
        Assert.Equal("Customer", model.TopProviders[1].Review!.CustomerName);
    }

    [Fact]
    public async Task OnGetAsync_AssignsDefaultTitleAndNullReviewWhenNotEnough()
    {
        var now = DateTime.UtcNow;
        await using var context = CreateContext(
            providers: new[]
            {
                CreateProvider(null, 4.0m, 10, 100),
                CreateProvider("  ", 3.0m, 5, 80)
            },
            reviews: new[]
            {
                CreateReview("Only", "Review", 5, now)
            });

        var model = new IndexModel(context);

        await model.OnGetAsync(CancellationToken.None);

        Assert.Equal("Service Provider", model.TopProviders[0].Title);
        Assert.Single(model.TopProviders, p => p.Review == null);
    }

    private static AppDbContext CreateContext(IEnumerable<ProviderProfile> providers, IEnumerable<CustomerReview> reviews)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        context.ProviderProfiles.AddRange(providers);
        context.CustomerReviews.AddRange(reviews);
        context.SaveChanges();
        return context;
    }

    private static ProviderProfile CreateProvider(string? title, decimal? ratingAverage, int? ratingCount, int? completedJobs) =>
        new()
        {
            Id = Guid.NewGuid(),
            ProfessionalTitle = string.IsNullOrEmpty(title) ? " " : title,
            RatingAverage = ratingAverage,
            RatingCount = ratingCount,
            CompletedJobsCount = completedJobs,
            HourlyRateAmount = 90m,
            CalloutFeeAmount = 30m,
            ServiceCategoriesCsv = "General",
            EmergencyAvailableFlag = true
        };

    private static CustomerReview CreateReview(string? name, string text, int rating, DateTime createdAt) =>
        new()
        {
            Id = Guid.NewGuid(),
            CustomerName = string.IsNullOrEmpty(name) ? " " : name,
            ReviewText = text,
            Rating = rating,
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = createdAt
        };
}
