using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class CustomerReviewFormTests
{
    [Fact]
    public void CustomerReviewForm_DefaultsAreInitialized()
    {
        var form = new CustomerReviewForm();

        Assert.Null(form.Id);
        Assert.Equal(5, form.Rating);
        Assert.Null(form.ImageUpload);
        Assert.Null(form.ExistingImagePath);
        Assert.False(form.RemoveImage);
    }

    [Fact]
    public void CustomerReviewForm_ValidModelPassesValidation()
    {
        var form = new CustomerReviewForm
        {
            CustomerName = "Alice",
            Rating = 4,
            ReviewText = "Great assistance!"
        };

        var (isValid, errors) = Validate(form);

        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Theory]
    [InlineData(null, 4, "Review text", nameof(CustomerReviewForm.CustomerName))]
    [InlineData("Alice", 0, "Review text", nameof(CustomerReviewForm.Rating))]
    [InlineData("Alice", 6, "Review text", nameof(CustomerReviewForm.Rating))]
    [InlineData("Alice", 4, null, nameof(CustomerReviewForm.ReviewText))]
    public void CustomerReviewForm_ValidationFailsForInvalidFields(string? name, int rating, string? reviewText, string expectedMember)
    {
        var form = new CustomerReviewForm
        {
            CustomerName = name,
            Rating = rating,
            ReviewText = reviewText
        };

        var (isValid, errors) = Validate(form);

        Assert.False(isValid);
        Assert.Contains(errors, e => e.MemberNames.Contains(expectedMember));
    }

    private static (bool IsValid, IList<ValidationResult> Errors) Validate(CustomerReviewForm form)
    {
        var context = new ValidationContext(form);
        var errors = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(form, context, errors, true);
        return (isValid, errors);
    }
}
