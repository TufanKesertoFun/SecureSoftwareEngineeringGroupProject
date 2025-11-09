using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Data;
using SecureSoftwareGroupProject.Model;
using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class ProviderProfilesModelTests
{
    [Fact]
    public async Task OnGet_LoadsProvidersOrderedByCreatedDate()
    {
        var now = DateTime.UtcNow;
        await using var context = CreateContext(new[]
        {
            CreateProvider("Old", now.AddDays(-2)),
            CreateProvider("Newest", now),
            CreateProvider("Middle", now.AddDays(-1))
        });

        var model = CreateModel(context);

        await model.OnGet();

        Assert.Equal(new[] { "Newest", "Middle", "Old" }, model.Rows.Select(r => r.ProfessionalTitle));
    }

    [Fact]
    public async Task OnGetLoad_ReturnsJsonWhenFound()
    {
        var existing = CreateProvider("Existing", DateTime.UtcNow);
        await using var context = CreateContext(new[] { existing });
        var model = CreateModel(context);

        var result = await model.OnGetLoad(existing.Id);

        var json = Assert.IsType<JsonResult>(result);
        var value = json.Value!;
        var titleProp = value.GetType().GetProperty("ProfessionalTitle");
        Assert.Equal("Existing", titleProp?.GetValue(value));
    }

    [Fact]
    public async Task OnPostCreate_InvalidModel_ReturnsPage()
    {
        await using var context = CreateContext(Array.Empty<ProviderProfile>());
        var model = CreateModel(context);
        model.ModelState.AddModelError("Form.ProfessionalTitle", "Required");

        var result = await model.OnPostCreate();

        Assert.IsType<PageResult>(result);
        Assert.Empty(context.ProviderProfiles);
    }

    [Fact]
    public async Task OnPostCreate_ValidModel_PersistsProvider()
    {
        await using var context = CreateContext(Array.Empty<ProviderProfile>());
        var model = CreateModel(context);
        model.Form = new ProviderForm
        {
            ProfessionalTitle = "Master Electrician",
            BusinessName = "ACME Power",
            YearsExperience = 12
        };

        var result = await model.OnPostCreate();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Null(redirect.PageName);
        Assert.Equal("Master Electrician", Assert.Single(context.ProviderProfiles).ProfessionalTitle);
    }

    [Fact]
    public async Task OnPostUpdate_InvalidModel_ReturnsPage()
    {
        var existing = CreateProvider("Existing", DateTime.UtcNow);
        await using var context = CreateContext(new[] { existing });
        var model = CreateModel(context);
        model.Form = new ProviderForm { Id = existing.Id, ProfessionalTitle = null };
        model.ModelState.AddModelError("Form.ProfessionalTitle", "Required");

        var result = await model.OnPostUpdate();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostUpdate_ValidModel_UpdatesEntity()
    {
        var existing = CreateProvider("Existing", DateTime.UtcNow);
        await using var context = CreateContext(new[] { existing });
        var model = CreateModel(context);
        model.Form = new ProviderForm
        {
            Id = existing.Id,
            ProfessionalTitle = "Updated",
            BusinessName = "Updated Biz"
        };

        var result = await model.OnPostUpdate();

        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Updated", context.ProviderProfiles.Single().ProfessionalTitle);
        Assert.Equal("Updated Biz", context.ProviderProfiles.Single().BusinessName);
    }

    [Fact]
    public async Task OnPostDelete_RemovesEntity()
    {
        var existing = CreateProvider("Existing", DateTime.UtcNow);
        await using var context = CreateContext(new[] { existing });
        var model = CreateModel(context);

        var result = await model.OnPostDelete(existing.Id);

        Assert.IsType<RedirectToPageResult>(result);
        Assert.Empty(context.ProviderProfiles);
    }

    private static ProviderProfilesModel CreateModel(AppDbContext context)
    {
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new PageActionDescriptor()
        };
        var pageContext = new PageContext(actionContext);

        var model = new ProviderProfilesModel(context)
        {
            PageContext = pageContext
        };

        return model;
    }

    private static AppDbContext CreateContext(IEnumerable<ProviderProfile> providers)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        context.ProviderProfiles.AddRange(providers);
        context.SaveChanges();
        return context;
    }

    private static ProviderProfile CreateProvider(string title, DateTime createdAt) =>
        new()
        {
            Id = Guid.NewGuid(),
            ProfessionalTitle = title,
            UserId = Guid.NewGuid(),
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = createdAt
        };
}
