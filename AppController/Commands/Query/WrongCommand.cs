using AppController.Commands;

namespace AppController.Commands.Query;

public class WrongCommand : ICommand
{
    public Task<CommandResult> ExecuteAsync(ParsedRequest request) =>
        Task.FromResult(CommandResult.Fail(
            $"Unknown command: '{request.CommandName}'. Type 'help' to see available commands."));
}
