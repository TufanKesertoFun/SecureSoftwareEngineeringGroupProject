using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Models;
using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class CustomerBalanceModelTests
{
    [Fact]
    public async Task OnGetAsync_LoadsBalancesOrderedByRegistrationDateDesc()
    {
        var now = DateTime.UtcNow;
        await using var context = CreateContext(new[]
        {
            CreateBalance(1, "Alice", now.AddDays(-1)),
            CreateBalance(2, "Bob", now.AddDays(-3)),
            CreateBalance(3, "Cara", now)
        });

        var pageModel = new CustomerBalanceModel(context);

        await pageModel.OnGetAsync();

        Assert.Equal(new[] { 3, 1, 2 }, pageModel.Balances.Select(b => b.Id));
    }

    private static AppDbContext CreateContext(IEnumerable<CustomerBalance> balances)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        context.CustomerBalances.AddRange(balances);
        context.SaveChanges();
        return context;
    }

    private static CustomerBalance CreateBalance(int id, string name, DateTime registrationDate) =>
        new()
        {
            Id = id,
            CustomerName = name,
            AccountNumber = $"ACC-{id:000}",
            RegistrationDate = registrationDate,
            LastPaymentDate = registrationDate.AddDays(-1),
            Balance = id * 10m,
            CreditLimit = 1000m,
            Currency = "USD"
        };
}
