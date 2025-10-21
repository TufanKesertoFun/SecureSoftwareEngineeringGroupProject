using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SecureSoftwareGroupProject.Pages
{
    public class CustomerReviewForm
    {
        public Guid? Id { get; set; }

        [Display(Name = "Customer"), Required, StringLength(150)]
        public string? CustomerName { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; } = 5;

        [Display(Name = "Review"), Required, StringLength(2000)]
        public string? ReviewText { get; set; }

        [Display(Name = "Review Image")]
        public IFormFile? ImageUpload { get; set; }

        public string? ExistingImagePath { get; set; }

        public bool RemoveImage { get; set; }
    }
}
