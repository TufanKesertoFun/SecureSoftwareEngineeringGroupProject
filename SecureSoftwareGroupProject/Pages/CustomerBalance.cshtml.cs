using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SecureSoftwareGroupProject.Models;

namespace SecureSoftwareGroupProject.Pages
{
    public class CustomerBalanceModel : PageModel
    {
        private readonly IConfiguration _config;
        public CustomerBalanceModel(IConfiguration config) => _config = config;

        public List<CustomerBalance> Balances { get; } = new();

        public void OnGet()
        {
            var cs = _config.GetConnectionString("DefaultConnection");

            using var cn = new SqlConnection(cs);
            cn.Open();

            const string sql = "SELECT * FROM dbo.CustomerBalance";
            using var cmd = new SqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                Balances.Add(new CustomerBalance
                {
                    Id = (int)rd["Id"],
                    CustomerName = (string)rd["CustomerName"],
                    AccountNumber = (string)rd["AccountNumber"],
                    Balance = (decimal)rd["Balance"],
                    Currency = (string)rd["Currency"],
                    LastPaymentDate = (DateTime)rd["LastPaymentDate"],
                    CreditLimit = (decimal)rd["CreditLimit"],
                    Status = (string)rd["Status"],
                    PhoneNumber = (string)rd["PhoneNumber"],
                    Email = (string)rd["Email"],
                    Address = (string)rd["Address"],
                    City = (string)rd["City"],
                    State = (string)rd["State"],
                    PostalCode = (string)rd["PostalCode"],
                    RegistrationDate = (DateTime)rd["RegistrationDate"]
                });
            }
        }
    }
}