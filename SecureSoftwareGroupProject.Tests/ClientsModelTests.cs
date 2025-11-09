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

public class ClientsModelTests
{
    [Fact]
    public async Task OnGetAsync_BuildsStatusOptionsAndClientSummaries()
    {
        var now = DateTime.UtcNow;
        using var context = CreateContext(new[]
        {
            CreateCustomer(1, "Acme Co", "Active", now),
            CreateCustomer(2, "Beta Corp", "Pending", now.AddDays(-1)),
            CreateCustomer(3, "Zenith Labs", null, now.AddDays(-2))
        });

        var pageModel = new ClientsModel(context);

        await pageModel.OnGetAsync(CancellationToken.None);

        Assert.Equal(new[] { "__all", "Active", "Pending", "__unspecified" }, pageModel.StatusOptions.Select(o => o.Value));
        Assert.Equal("__all", pageModel.Status);
        Assert.Equal(new[] { "Acme Co", "Beta Corp", "Zenith Labs" }, pageModel.Clients.Select(c => c.Name));
        Assert.Equal("Unspecified", pageModel.Clients.Single(c => c.Name == "Zenith Labs").Status);
    }

    [Fact]
    public async Task OnGetAsync_FiltersByStatusAndSearchTerm()
    {
        var now = DateTime.UtcNow;
        using var context = CreateContext(new[]
        {
            CreateCustomer(1, "Acme Co", "Active", now),
            CreateCustomer(2, "Beta Corp", null, now.AddDays(-1)),
            CreateCustomer(3, "Gamma Bank", null, now.AddDays(-3))
        });

        var pageModel = new ClientsModel(context)
        {
            Status = "__unspecified",
            Search = "beta"
        };

        await pageModel.OnGetAsync(CancellationToken.None);

        Assert.Equal("__unspecified", pageModel.Status);
        Assert.Single(pageModel.Clients);
        Assert.Equal("Beta Corp", pageModel.Clients[0].Name);
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

    private static CustomerBalance CreateCustomer(int id, string name, string? status, DateTime registeredUtc) =>
        new()
        {
            Id = id,
            CustomerName = name,
            AccountNumber = $"ACC-{id:000}",
            Status = status,
            Email = $"{name.Replace(" ", "").ToLowerInvariant()}@example.com",
            PhoneNumber = "555-0000",
            City = "Metropolis",
            State = "NY",
            LastPaymentDate = registeredUtc.AddDays(-5),
            RegistrationDate = registeredUtc,
            Balance = id * 100m,
            CreditLimit = 1000m
        };
}
