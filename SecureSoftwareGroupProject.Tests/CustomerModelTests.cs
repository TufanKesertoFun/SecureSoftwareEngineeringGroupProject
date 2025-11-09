using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Models;
using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class CustomerModelTests
{
    [Fact]
    public async Task OnGetAsync_PopulatesCurrenciesAndRowsWithDefaultFilters()
    {
        var now = DateTime.UtcNow;
        await using var context = CreateContext(new[]
        {
            CreateCustomer(1, "Alice", "Open", "USD", 100m, now),
            CreateCustomer(2, "Bob", "Closed", "EUR", 200m, now.AddDays(-1)),
            CreateCustomer(3, "Cara", "Active", null, 300m, now.AddDays(-2))
        });

        var pageModel = new CustomerModel(context);
        await pageModel.OnGetAsync(CancellationToken.None);

        Assert.Equal(new[] { "EUR", "USD" }, pageModel.AllCurrencies);
        Assert.Equal(new[] { "Alice", "Bob", "Cara" }, pageModel.Rows.Select(r => r.CustomerName));
        Assert.Equal(new[] { "Open", "Closed", "Active" }, pageModel.Rows.Select(r => r.Status));
    }

    [Fact]
    public async Task OnGetAsync_AppliesFilters()
    {
        var now = DateTime.UtcNow;
        await using var context = CreateContext(new[]
        {
            CreateCustomer(1, "Alice", "Active", "USD", 50m, now),
            CreateCustomer(2, "Bob", "Active", "EUR", 500m, now.AddDays(-1)),
            CreateCustomer(3, "Carl", "Inactive", "USD", 250m, now.AddDays(-2)),
            CreateCustomer(4, "Dora", "Active", "USD", 700m, now.AddDays(-3))
        });

        var pageModel = new CustomerModel(context)
        {
            Status = "Active",
            Currency = "USD",
            MinBalance = 200m,
            Query = "dor"
        };

        await pageModel.OnGetAsync(CancellationToken.None);

        Assert.Single(pageModel.Rows);
        var row = pageModel.Rows[0];
        Assert.Equal("Dora", row.CustomerName);
        Assert.Equal("USD", row.Currency);
        Assert.Equal("Active", row.Status);
    }

    private static AppDbContext CreateContext(IEnumerable<CustomerBalance> customerBalances)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        context.CustomerBalances.AddRange(customerBalances);
        context.SaveChanges();
        return context;
    }

    private static CustomerBalance CreateCustomer(int id, string name, string? status, string? currency, decimal balance, DateTime lastPayment) =>
        new()
        {
            Id = id,
            CustomerName = name,
            AccountNumber = $"ACC-{id:000}",
            Status = status,
            Currency = currency,
            Balance = balance,
            CreditLimit = 1000m,
            LastPaymentDate = lastPayment,
            RegistrationDate = lastPayment.AddDays(-30),
            Email = $"{name.ToLowerInvariant()}@example.com",
            City = "Metropolis",
            PhoneNumber = "555-1234"
        };
}
