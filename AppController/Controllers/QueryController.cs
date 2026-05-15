using AppController.Providers;
using AppController.Utils;
using Microsoft.Extensions.Logging;

namespace AppController.Controllers;

public class QueryController(
    CommandRegistry registry,
    RequestParser parser,
    ILogger<QueryController> logger)
{
    public async Task<CommandResult> ExecuteAsync(string input)
    {
        var request = parser.Parse(input);
        logger.LogInformation("Executing command: {Command}", request.CommandName);

        var result = await registry.Resolve(request).ExecuteAsync(request);

        if (!result.Success)
            logger.LogWarning("Command {Command} failed: {Message}", request.CommandName, result.Message);

        return result;
    }

    public async Task<CommandResult> ExecuteWithArgsAsync(string commandName, Dictionary<string, string> args)
    {
        var request = parser.ParseWithArgs(commandName, args);
        logger.LogInformation("Executing command: {Command}", request.CommandName);

        var result = await registry.Resolve(request).ExecuteAsync(request);

        if (!result.Success)
            logger.LogWarning("Command {Command} failed: {Message}", request.CommandName, result.Message);

        return result;
    }
}
