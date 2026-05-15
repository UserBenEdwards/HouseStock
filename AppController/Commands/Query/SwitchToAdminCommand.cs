using AppController.Commands;
using Microsoft.Extensions.Logging;
using Service.Interfaces;

namespace AppController.Commands.Query;

public class SwitchToAdminCommand(
    IAuthService authService,
    AppSession session,
    ILogger<SwitchToAdminCommand> logger) : ICommand
{
    public Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        var password = request.Get("password") ?? string.Empty;

        if (!authService.Authenticate(password))
        {
            logger.LogWarning("Failed admin login attempt");
            return Task.FromResult(CommandResult.Fail("Incorrect password."));
        }

        session.SwitchToAdmin();
        logger.LogInformation("Session switched to admin mode");
        return Task.FromResult(CommandResult.Ok("Switched to admin mode."));
    }
}
