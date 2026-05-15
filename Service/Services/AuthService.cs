using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Interfaces;
using Service.Options;

namespace Service.Services;

public class AuthService(
    IOptions<AuthOptions> options,
    ILogger<AuthService> logger) : IAuthService
{
    public bool Authenticate(string password)
    {
        if (password == options.Value.AdminPassword)
            return true;

        logger.LogWarning("Failed authentication attempt");
        return false;
    }
}
