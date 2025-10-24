using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecureSoftwareGroupProject.Model
{
    [Table("ProviderProfile")]
    public class ProviderProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Display(Name = "User Id")]
        public Guid UserId { get; set; }

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

        public string? CertificationsCsv { get; set; }

        public string? ServiceCategoriesCsv { get; set; }

        public string? ServiceAreaCodesCsv { get; set; }

        public string? LanguagesCsv { get; set; }

        public bool? EmergencyAvailableFlag { get; set; }

        public string? VehicleType { get; set; }

        public string? ToolsOwnedCsv { get; set; }

        public string? InsuranceProvider { get; set; }

        public string? InsurancePolicyNumber { get; set; }

        public DateTime? InsuranceExpiryDateUtc { get; set; }

        public string? VerificationLevel { get; set; }

        public decimal? RatingAverage { get; set; }

        public int? RatingCount { get; set; }

        public int? CompletedJobsCount { get; set; }

        public int? ResponseTimeMinutesAvg { get; set; }

        public string? IntroVideoUrl { get; set; }

        public DateTime? CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
