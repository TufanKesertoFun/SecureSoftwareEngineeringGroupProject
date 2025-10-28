using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Model;   // ProviderProfile entity
using SecureSoftwareGroupProject.Pages;   // ProviderForm DTO

namespace SecureSoftwareGroupProject.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ProviderProfilesModel : PageModel
    {
        private readonly AppDbContext _db;
        public ProviderProfilesModel(AppDbContext db) => _db = db;

        // 🔹 this is what your .cshtml expects
        public List<ProviderProfile> Rows { get; private set; } = new();

        [BindProperty]
        public ProviderForm Form { get; set; } = new();

        // -------- GET: list providers ----------
        public async Task OnGet()
        {
            Rows = await _db.Set<ProviderProfile>()
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync();
        }

        // -------- AJAX: load single provider ----------
        public async Task<IActionResult> OnGetLoad(Guid id)
        {
            var p = await _db.Set<ProviderProfile>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return NotFound();

            return new JsonResult(new
            {
                p.Id,
                p.ProfessionalTitle,
                p.BusinessName,
                p.YearsExperience,
                p.HourlyRateAmount,
                p.CalloutFeeAmount,
                p.SkillsCsv,
                p.ServiceCategoriesCsv,
                p.EmergencyAvailableFlag
            });
        }

        // -------- POST: create ----------
        public async Task<IActionResult> OnPostCreate()
        {
            if (!ModelState.IsValid)
            {
                await OnGet();
                return Page();
            }

            var entity = new ProviderProfile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Empty, // replace with current user if you have auth
                ProfessionalTitle = Form.ProfessionalTitle!,
                BusinessName = Form.BusinessName,
                YearsExperience = Form.YearsExperience,
                HourlyRateAmount = Form.HourlyRateAmount,
                CalloutFeeAmount = Form.CalloutFeeAmount,
                SkillsCsv = Form.SkillsCsv,
                ServiceCategoriesCsv = Form.ServiceCategoriesCsv,
                EmergencyAvailableFlag = Form.EmergencyAvailableFlag,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _db.Add(entity);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        // -------- POST: update ----------
        public async Task<IActionResult> OnPostUpdate()
        {
            if (!ModelState.IsValid || Form.Id == null)
            {
                await OnGet();
                return Page();
            }

            var p = await _db.Set<ProviderProfile>().FirstOrDefaultAsync(x => x.Id == Form.Id.Value);
            if (p == null) return NotFound();

            p.ProfessionalTitle = Form.ProfessionalTitle!;
            p.BusinessName = Form.BusinessName;
            p.YearsExperience = Form.YearsExperience;
            p.HourlyRateAmount = Form.HourlyRateAmount;
            p.CalloutFeeAmount = Form.CalloutFeeAmount;
            p.SkillsCsv = Form.SkillsCsv;
            p.ServiceCategoriesCsv = Form.ServiceCategoriesCsv;
            p.EmergencyAvailableFlag = Form.EmergencyAvailableFlag;
            p.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        // -------- POST: delete ----------
        public async Task<IActionResult> OnPostDelete(Guid id)
        {
            var p = await _db.Set<ProviderProfile>().FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();

            _db.Remove(p);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
