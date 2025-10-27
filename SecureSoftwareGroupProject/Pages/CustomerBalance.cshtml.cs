using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Models;

namespace SecureSoftwareGroupProject.Pages
{
    public class CustomerBalanceModel : PageModel
    {
        private readonly AppDbContext _db;
        public CustomerBalanceModel(AppDbContext db) => _db = db;

        public List<CustomerBalance> Balances { get; } = new();

        public async Task OnGetAsync()
        {
            Balances.AddRange(
                await _db.CustomerBalances
                    .AsNoTracking()
                    .OrderByDescending(c => c.RegistrationDate)
                    .ToListAsync());
        }
    }
}
