using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SecureSoftwareGroupProject.Model;

namespace SecureSoftwareGroupProject.Tests;

public class SignupDtoTests
{
    [Fact]
    public void SignupDto_ValidModelPassesValidation()
    {
        var dto = new SignupDto
        {
            Username = "secure_user",
            Password = "StrongPass123",
            ConfirmPassword = "StrongPass123"
        };

        var (isValid, errors) = Validate(dto);

        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Theory]
    [InlineData("", "StrongPass123", "StrongPass123", nameof(SignupDto.Username))]
    [InlineData("ab", "StrongPass123", "StrongPass123", nameof(SignupDto.Username))]
    [InlineData("validuser", "", "", nameof(SignupDto.Password))]
    [InlineData("validuser", "short", "short", nameof(SignupDto.Password))]
    public void SignupDto_ValidationFailsForInvalidLengths(string username, string password, string confirmPassword, string expectedMember)
    {
        var dto = new SignupDto
        {
            Username = username,
            Password = password,
            ConfirmPassword = confirmPassword
        };

        var (isValid, errors) = Validate(dto);

        Assert.False(isValid);
        Assert.Contains(errors, e => e.MemberNames.Contains(expectedMember));
    }

    [Fact]
    public void SignupDto_ValidationFailsWhenPasswordsDoNotMatch()
    {
        var dto = new SignupDto
        {
            Username = "validuser",
            Password = "StrongPass123",
            ConfirmPassword = "Mismatch456"
        };

        var (isValid, errors) = Validate(dto);

        Assert.False(isValid);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(SignupDto.ConfirmPassword)));
    }

    private static (bool IsValid, IList<ValidationResult> Errors) Validate(SignupDto dto)
    {
        var context = new ValidationContext(dto);
        var errors = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, errors, true);
        return (isValid, errors);
    }
}
