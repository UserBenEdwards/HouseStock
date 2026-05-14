using Microsoft.Extensions.Options;
using Moq;
using Service.Options;
using Service.Services;

namespace Test.Services;

public class AuthServiceTests
{
    private static AuthService CreateService(string adminPassword)
    {
        var options = new Mock<IOptions<AuthOptions>>();
        options.Setup(o => o.Value).Returns(new AuthOptions { AdminPassword = adminPassword });
        return new AuthService(options.Object);
    }

    [Fact]
    public void Authenticate_WithCorrectPassword_ReturnsTrue()
    {
        var service = CreateService("admin123");

        var result = service.Authenticate("admin123");

        Assert.True(result);
    }

    [Fact]
    public void Authenticate_WithWrongPassword_ReturnsFalse()
    {
        var service = CreateService("admin123");

        var result = service.Authenticate("wrong");

        Assert.False(result);
    }

    [Fact]
    public void Authenticate_WithEmptyPassword_ReturnsFalse()
    {
        var service = CreateService("admin123");

        var result = service.Authenticate(string.Empty);

        Assert.False(result);
    }

    [Theory]
    [InlineData("Admin123")]
    [InlineData("ADMIN123")]
    [InlineData(" admin123")]
    public void Authenticate_WithWrongCaseOrWhitespace_ReturnsFalse(string password)
    {
        var service = CreateService("admin123");

        var result = service.Authenticate(password);

        Assert.False(result);
    }
}
