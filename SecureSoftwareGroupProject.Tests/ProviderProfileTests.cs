using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SecureSoftwareGroupProject.Model;

namespace SecureSoftwareGroupProject.Tests;

public class ProviderProfileTests
{
    [Fact]
    public void ProviderProfile_DefaultsAreInitialized()
    {
        var beforeCreation = DateTime.UtcNow;
        var profile = new ProviderProfile();
        var afterCreation = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, profile.Id);
        Assert.Null(profile.BusinessName);
        Assert.Null(profile.YearsExperience);
        Assert.NotNull(profile.CreatedAtUtc);
        Assert.NotNull(profile.UpdatedAtUtc);
        Assert.InRange(profile.CreatedAtUtc.Value, beforeCreation, afterCreation);
        Assert.InRange(profile.UpdatedAtUtc.Value, beforeCreation, afterCreation);
    }

    [Fact]
    public void ProviderProfile_ValidationSucceedsForValidProfile()
    {
        var profile = new ProviderProfile
        {
            UserId = Guid.NewGuid(),
            ProfessionalTitle = "Certified Field Technician",
            BusinessName = "Quick Fixers LLC",
            YearsExperience = 10,
            HourlyRateAmount = 95.50m,
            EmergencyAvailableFlag = true
        };

        var (isValid, errors) = Validate(profile);

        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Fact]
    public void ProviderProfile_ValidationFailsWhenProfessionalTitleMissing()
    {
        var profile = new ProviderProfile
        {
            UserId = Guid.NewGuid(),
            ProfessionalTitle = null,
            YearsExperience = 5
        };

        var (isValid, errors) = Validate(profile);

        Assert.False(isValid);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ProviderProfile.ProfessionalTitle)));
    }

    [Fact]
    public void ProviderProfile_ValidationFailsForYearsExperienceOutOfRange()
    {
        var profile = new ProviderProfile
        {
            UserId = Guid.NewGuid(),
            ProfessionalTitle = "Lead Electrician",
            YearsExperience = 99
        };

        var (isValid, errors) = Validate(profile);

        Assert.False(isValid);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ProviderProfile.YearsExperience)));
    }

    private static (bool IsValid, IList<ValidationResult> Errors) Validate(ProviderProfile profile)
    {
        var context = new ValidationContext(profile);
        var errors = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(profile, context, errors, true);
        return (isValid, errors);
    }
}
