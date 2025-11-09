using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class ProviderFormTests
{
    [Fact]
    public void ProviderForm_ValidData_PassesValidation()
    {
        var form = new ProviderForm
        {
            ProfessionalTitle = "Certified Electrician",
            BusinessName = "Bright Sparks",
            YearsExperience = 10,
            HourlyRateAmount = 120m,
            CalloutFeeAmount = 50m,
            SkillsCsv = "Wiring,Inspection",
            ServiceCategoriesCsv = "Residential",
            EmergencyAvailableFlag = true
        };

        var (isValid, errors) = Validate(form);

        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Theory]
    [InlineData(null, 5, nameof(ProviderForm.ProfessionalTitle))]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", 5, nameof(ProviderForm.ProfessionalTitle))]
    [InlineData("Certified Electrician", 70, nameof(ProviderForm.YearsExperience))]
    public void ProviderForm_InvalidFields_ReturnValidationErrors(string? title, int? yearsExperience, string expectedMember)
    {
        var form = new ProviderForm
        {
            ProfessionalTitle = title,
            YearsExperience = yearsExperience
        };

        var (isValid, errors) = Validate(form);

        Assert.False(isValid);
        Assert.Contains(errors, e => e.MemberNames.Contains(expectedMember));
    }

    private static (bool IsValid, IList<ValidationResult> Errors) Validate(ProviderForm form)
    {
        var context = new ValidationContext(form);
        var errors = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(form, context, errors, true);
        return (isValid, errors);
    }
}
