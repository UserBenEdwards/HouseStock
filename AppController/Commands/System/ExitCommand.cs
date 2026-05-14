using AppController.Commands;

namespace AppController.Commands.System;

public class ExitCommand : ICommand
{
    public Task<CommandResult> ExecuteAsync(ParsedRequest request) =>
        Task.FromResult(CommandResult.Exit());
}
