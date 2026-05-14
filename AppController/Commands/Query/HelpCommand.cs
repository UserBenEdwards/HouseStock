using AppController.Commands;

namespace AppController.Commands.Query;

public class HelpCommand : ICommand
{
    private const string HelpText = """
        === Available Commands ===

        Query commands:
          find all                  — Find all appliances
          find <category>           — Find appliances by category
          find all price=min;max    — Find appliances by price range
          cost <min> <max>          — Find appliances by price range
          show <id>                 — Show appliance details by ID

        Mode commands:
          switch admin              — Switch to admin mode

        Other commands:
          help                      — Show this help
          exit                      — Exit application
        """;

    public Task<CommandResult> ExecuteAsync(ParsedRequest request) =>
        Task.FromResult(CommandResult.Ok(HelpText));
}
