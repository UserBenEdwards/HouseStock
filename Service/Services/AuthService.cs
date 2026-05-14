using Microsoft.Extensions.Options;
using Service.Interfaces;
using Service.Options;

namespace Service.Services;

public class AuthService(IOptions<AuthOptions> options) : IAuthService
{
    public bool Authenticate(string password) =>
        password == options.Value.AdminPassword;
}
