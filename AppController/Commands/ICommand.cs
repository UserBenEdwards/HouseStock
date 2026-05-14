namespace AppController.Commands;

public interface ICommand
{
    Task<CommandResult> ExecuteAsync(ParsedRequest request);
}
