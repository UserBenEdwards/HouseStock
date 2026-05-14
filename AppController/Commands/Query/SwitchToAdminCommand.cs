using AppController.Commands;
using Service.Interfaces;

namespace AppController.Commands.Query;

public class SwitchToAdminCommand(IAuthService authService, AppSession session) : ICommand
{
    public Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        var password = request.Get("password") ?? string.Empty;

        if (!authService.Authenticate(password))
            return Task.FromResult(CommandResult.Fail("Incorrect password."));

        session.SwitchToAdmin();
        return Task.FromResult(CommandResult.Ok("Switched to admin mode."));
    }
}
