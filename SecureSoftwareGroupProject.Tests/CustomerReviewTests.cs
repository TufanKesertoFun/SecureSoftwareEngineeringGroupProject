using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SecureSoftwareGroupProject.Models;

namespace SecureSoftwareGroupProject.Tests;

public class CustomerReviewTests
{
    [Fact]
    public void CustomerReview_DefaultsAreInitialized()
    {
        var beforeCreation = DateTime.UtcNow;
        var review = new CustomerReview();
        var afterCreation = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, review.Id);
        Assert.Equal(string.Empty, review.CustomerName);
        Assert.Equal(string.Empty, review.ReviewText);
        Assert.Null(review.ImagePath);
        Assert.InRange(review.CreatedAtUtc, beforeCreation, afterCreation);
        Assert.InRange(review.UpdatedAtUtc, beforeCreation, afterCreation);
    }

    [Fact]
    public void CustomerReview_ValidationSucceedsForValidReview()
    {
        var review = new CustomerReview
        {
            CustomerName = "Alex Doe",
            Rating = 4,
            ReviewText = "Great experience with rapid response times.",
            ImagePath = "/images/review.png"
        };

        var context = new ValidationContext(review);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(review, context, validationResults, true);

        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void CustomerReview_ValidationFailsForOutOfRangeRating(int rating)
    {
        var review = new CustomerReview
        {
            CustomerName = "Jamie Doe",
            Rating = rating,
            ReviewText = "Support could be better."
        };

        var context = new ValidationContext(review);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(review, context, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, result =>
            result.MemberNames.Contains(nameof(CustomerReview.Rating)));
    }
}
