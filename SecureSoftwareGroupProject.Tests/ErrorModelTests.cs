using SecureSoftwareGroupProject.Pages;

namespace SecureSoftwareGroupProject.Tests;

public class ErrorModelTests
{
    [Fact]
    public void ShowRequestId_ReturnsFalse_WhenRequestIdMissing()
    {
        var model = new ErrorModel();

        Assert.False(model.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_ReturnsTrue_WhenRequestIdProvided()
    {
        var model = new ErrorModel { RequestId = "trace-id" };

        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void OnGet_DoesNotMutateRequestId()
    {
        var model = new ErrorModel { RequestId = "trace-id" };

        model.OnGet();

        Assert.Equal("trace-id", model.RequestId);
    }
}
