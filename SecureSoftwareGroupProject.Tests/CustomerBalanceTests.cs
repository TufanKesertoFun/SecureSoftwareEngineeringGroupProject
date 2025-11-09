using System;
using SecureSoftwareGroupProject.Models;

namespace SecureSoftwareGroupProject.Tests;

public class CustomerBalanceTests
{
    [Fact]
    public void CustomerBalance_PersistsAssignedValues()
    {
        var registrationDate = new DateTime(2024, 02, 15, 10, 30, 00, DateTimeKind.Utc);
        var lastPaymentDate = registrationDate.AddDays(-5);

        var customerBalance = new CustomerBalance
        {
            Id = 7,
            CustomerName = "Jane Doe",
            AccountNumber = "ACC-001",
            Balance = 1250.75m,
            Currency = "USD",
            LastPaymentDate = lastPaymentDate,
            CreditLimit = 5000m,
            Status = "Active",
            PhoneNumber = "+1-555-1234",
            Email = "jane.doe@example.com",
            Address = "123 Main St",
            City = "Metropolis",
            State = "NY",
            PostalCode = "12345",
            RegistrationDate = registrationDate
        };

        Assert.Equal(7, customerBalance.Id);
        Assert.Equal("Jane Doe", customerBalance.CustomerName);
        Assert.Equal("ACC-001", customerBalance.AccountNumber);
        Assert.Equal(1250.75m, customerBalance.Balance);
        Assert.Equal("USD", customerBalance.Currency);
        Assert.Equal(lastPaymentDate, customerBalance.LastPaymentDate);
        Assert.Equal(5000m, customerBalance.CreditLimit);
        Assert.Equal("Active", customerBalance.Status);
        Assert.Equal("+1-555-1234", customerBalance.PhoneNumber);
        Assert.Equal("jane.doe@example.com", customerBalance.Email);
        Assert.Equal("123 Main St", customerBalance.Address);
        Assert.Equal("Metropolis", customerBalance.City);
        Assert.Equal("NY", customerBalance.State);
        Assert.Equal("12345", customerBalance.PostalCode);
        Assert.Equal(registrationDate, customerBalance.RegistrationDate);
    }

    [Fact]
    public void CustomerBalance_DefaultsRemainUnsetUntilChanged()
    {
        var customerBalance = new CustomerBalance
        {
            Id = 1,
            CustomerName = "John Smith",
            AccountNumber = "ACC-002"
        };

        Assert.Equal(0m, customerBalance.Balance);
        Assert.Null(customerBalance.Currency);
        Assert.Equal(default, customerBalance.LastPaymentDate);
        Assert.Equal(0m, customerBalance.CreditLimit);
        Assert.Null(customerBalance.Status);
        Assert.Null(customerBalance.PhoneNumber);
        Assert.Null(customerBalance.Email);
        Assert.Null(customerBalance.Address);
        Assert.Null(customerBalance.City);
        Assert.Null(customerBalance.State);
        Assert.Null(customerBalance.PostalCode);
        Assert.Equal(default, customerBalance.RegistrationDate);
    }
}
