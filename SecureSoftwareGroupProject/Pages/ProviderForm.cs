using System;
using System.ComponentModel.DataAnnotations;

namespace SecureSoftwareGroupProject.Pages
{
    // Top-level DTO the page and the partial can both see
    public class ProviderForm
    {
        public Guid? Id { get; set; }

        [Required, StringLength(120)]
        public string? ProfessionalTitle { get; set; }

        [StringLength(160)]
        public string? BusinessName { get; set; }

        [Range(0, 60)]
        public int? YearsExperience { get; set; }

        [DataType(DataType.Currency)]
        public decimal? HourlyRateAmount { get; set; }

        [DataType(DataType.Currency)]
        public decimal? CalloutFeeAmount { get; set; }

        public string? SkillsCsv { get; set; }
        public string? ServiceCategoriesCsv { get; set; }
        public bool? EmergencyAvailableFlag { get; set; }
    }
}
