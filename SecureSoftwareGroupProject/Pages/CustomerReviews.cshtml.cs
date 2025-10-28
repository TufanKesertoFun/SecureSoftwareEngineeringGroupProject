using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Models;

namespace SecureSoftwareGroupProject.Pages
{
    public class CustomerReviewsModel : PageModel
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxImageBytes = 2 * 1024 * 1024; // 2 MB

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public CustomerReviewsModel(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public List<CustomerReview> Reviews { get; private set; } = new();
        public List<string> ActiveCustomers { get; private set; } = new();

        [BindProperty]
        public CustomerReviewForm Form { get; set; } = new();

        public async Task OnGet()
        {
            await LoadPageAsync();
        }

        public async Task<IActionResult> OnGetLoad(Guid id)
        {
            var review = await _db.CustomerReviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                review.Id,
                review.CustomerName,
                review.Rating,
                review.ReviewText,
                review.ImagePath
            });
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            await LoadActiveCustomersAsync();

            if (!IsKnownActiveCustomer(Form.CustomerName))
            {
                ModelState.AddModelError("Form.CustomerName", "Select a customer marked as Active.");
            }

            var imageError = ValidateImage(Form.ImageUpload);
            if (!string.IsNullOrEmpty(imageError))
            {
                ModelState.AddModelError("Form.ImageUpload", imageError);
            }

            if (!ModelState.IsValid)
            {
                await LoadReviewsAsync();
                return Page();
            }

            string? imagePath = null;
            if (Form.ImageUpload != null)
            {
                imagePath = await SaveImageAsync(Form.ImageUpload);
            }

            var entity = new CustomerReview
            {
                CustomerName = Form.CustomerName!.Trim(),
                Rating = Form.Rating,
                ReviewText = Form.ReviewText!.Trim(),
                ImagePath = imagePath,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _db.CustomerReviews.Add(entity);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            await LoadActiveCustomersAsync();

            if (Form.Id == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to locate the review to update.");
            }

            if (!IsKnownActiveCustomer(Form.CustomerName))
            {
                ModelState.AddModelError("Form.CustomerName", "Select a customer marked as Active.");
            }

            var imageError = ValidateImage(Form.ImageUpload);
            if (!string.IsNullOrEmpty(imageError))
            {
                ModelState.AddModelError("Form.ImageUpload", imageError);
            }

            if (!ModelState.IsValid)
            {
                await LoadReviewsAsync();
                return Page();
            }

            var review = await _db.CustomerReviews.FirstOrDefaultAsync(r => r.Id == Form.Id);
            if (review == null)
            {
                return NotFound();
            }

            review.CustomerName = Form.CustomerName!.Trim();
            review.Rating = Form.Rating;
            review.ReviewText = Form.ReviewText!.Trim();
            review.UpdatedAtUtc = DateTime.UtcNow;

            if (Form.RemoveImage)
            {
                DeleteImageIfExists(review.ImagePath);
                review.ImagePath = null;
            }
            else if (Form.ImageUpload != null)
            {
                var newImagePath = await SaveImageAsync(Form.ImageUpload);
                DeleteImageIfExists(review.ImagePath);
                review.ImagePath = newImagePath;
            }

            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var review = await _db.CustomerReviews.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            DeleteImageIfExists(review.ImagePath);
            _db.CustomerReviews.Remove(review);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        private async Task LoadPageAsync()
        {
            await LoadActiveCustomersAsync();
            await LoadReviewsAsync();
        }

        private async Task LoadActiveCustomersAsync()
        {
            ActiveCustomers = await _db.CustomerBalances
                .AsNoTracking()
                .Where(c => c.Status == "Active" && c.CustomerName != null)
                .Select(c => c.CustomerName)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();
        }

        private async Task LoadReviewsAsync()
        {
            Reviews = await _db.CustomerReviews
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync();
        }

        private bool IsKnownActiveCustomer(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return ActiveCustomers.Any(n =>
                string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
        }

        private static string? ValidateImage(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null)
            {
                return null;
            }

            if (file.Length == 0)
            {
                return "The uploaded file was empty.";
            }

            if (file.Length > MaxImageBytes)
            {
                return "Images must be 2 MB or smaller.";
            }

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext) ||
                !AllowedExtensions.Contains(ext.ToLowerInvariant()))
            {
                return "Only JPG, PNG, GIF, or WEBP files are allowed.";
            }

            return null;
        }

        private async Task<string> SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile file)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "customer-reviews");
            Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(file.FileName)!.ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            await using var stream = System.IO.File.Create(fullPath);
            await file.CopyToAsync(stream);

            return $"/uploads/customer-reviews/{fileName}";
        }

        private void DeleteImageIfExists(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            var trimmed = relativePath.TrimStart('~', '/')
                .Replace('/', Path.DirectorySeparatorChar);
            var root = Path.GetFullPath(_env.WebRootPath);
            var fullPath = Path.GetFullPath(Path.Combine(_env.WebRootPath, trimmed));

            if (!fullPath.StartsWith(root, StringComparison.Ordinal))
            {
                return;
            }

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
