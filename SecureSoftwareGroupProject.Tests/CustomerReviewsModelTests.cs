using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Models;
using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class CustomerReviewsModelTests
{
    [Fact]
    public async Task OnGet_LoadsActiveCustomersAndReviews()
    {
        var now = DateTime.UtcNow;
        await using var context = CreateContext(
            customerBalances: new[]
            {
                CreateCustomerBalance("Acme", "Active"),
                CreateCustomerBalance("Beta", "Inactive"),
                CreateCustomerBalance("Zenith", "Active")
            },
            customerReviews: new[]
            {
                CreateCustomerReview("Oldest", now.AddDays(-2)),
                CreateCustomerReview("Newest", now)
            });

        var model = CreateModel(context);

        await model.OnGet();

        Assert.Equal(new[] { "Acme", "Zenith" }, model.ActiveCustomers);
        Assert.Equal(new[] { "Newest", "Oldest" }, model.Reviews.Select(r => r.CustomerName));
    }

    [Fact]
    public async Task OnPostCreate_InvalidCustomerReturnsPageResult()
    {
        await using var context = CreateContext(
            customerBalances: new[] { CreateCustomerBalance("Acme", "Active") });

        var model = CreateModel(context);
        model.Form = new CustomerReviewForm
        {
            CustomerName = "Unknown",
            Rating = 5,
            ReviewText = "Test"
        };

        var result = await model.OnPostCreateAsync();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.Empty(context.CustomerReviews);
    }

    [Fact]
    public async Task OnPostCreate_SavesReview_WhenFormValid()
    {
        await using var context = CreateContext(
            customerBalances: new[] { CreateCustomerBalance("Acme", "Active") });

        var model = CreateModel(context);
        model.Form = new CustomerReviewForm
        {
            CustomerName = "Acme",
            Rating = 4,
            ReviewText = " Excellent service "
        };

        var result = await model.OnPostCreateAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Null(redirect.PageName); // default reload

        var saved = context.CustomerReviews.Single();
        Assert.Equal("Acme", saved.CustomerName);
        Assert.Equal("Excellent service", saved.ReviewText);
        Assert.Equal(4, saved.Rating);
    }

    private static CustomerReviewsModel CreateModel(AppDbContext context)
    {
        return new CustomerReviewsModel(context, new FakeWebHostEnvironment());
    }

    private static AppDbContext CreateContext(IEnumerable<CustomerBalance> customerBalances, IEnumerable<CustomerReview>? customerReviews = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        context.CustomerBalances.AddRange(customerBalances);
        if (customerReviews != null)
        {
            context.CustomerReviews.AddRange(customerReviews);
        }

        context.SaveChanges();
        return context;
    }

    private static CustomerBalance CreateCustomerBalance(string name, string status) =>
        new()
        {
            CustomerName = name,
            Status = status,
            AccountNumber = Guid.NewGuid().ToString(),
            CreditLimit = 1000m,
            Balance = 500m,
            LastPaymentDate = DateTime.UtcNow.AddDays(-1),
            RegistrationDate = DateTime.UtcNow.AddDays(-30)
        };

    private static CustomerReview CreateCustomerReview(string customerName, DateTime createdAt) =>
        new()
        {
            CustomerName = customerName,
            Rating = 5,
            ReviewText = "Sample",
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = createdAt
        };

    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public FakeWebHostEnvironment()
        {
            var temp = Path.Combine(Path.GetTempPath(), "CustomerReviewsTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temp);
            WebRootPath = temp;
            ContentRootPath = temp;
        }

        public string ApplicationName { get; set; } = "Tests";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; } = "Development";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; }
    }
}
