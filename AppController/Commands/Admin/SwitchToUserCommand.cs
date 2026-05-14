using AppController.Commands;

namespace AppController.Commands.Admin;

public class SwitchToUserCommand(AppSession session) : ICommand
{
    public Task<CommandResult> ExecuteAsync(ParsedRequest request)
    {
        session.SwitchToUser();
        return Task.FromResult(CommandResult.Ok("Switched to user mode."));
    }
}
