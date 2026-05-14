using AppController.Providers;
using AppController.Utils;

namespace AppController.Controllers;

public class QueryController(CommandRegistry registry, RequestParser parser)
{
    public async Task<CommandResult> ExecuteAsync(string input)
    {
        var request = parser.Parse(input);
        var command = registry.Resolve(request);
        return await command.ExecuteAsync(request);
    }

    public async Task<CommandResult> ExecuteWithArgsAsync(string commandName, Dictionary<string, string> args)
    {
        var request = parser.ParseWithArgs(commandName, args);
        var command = registry.Resolve(request);
        return await command.ExecuteAsync(request);
    }
}
