using AppController.Commands;

namespace AppController.Commands.Admin;

public class AdminHelpCommand : ICommand
{
    private const string HelpText = """
        === Available Commands (Admin Mode) ===

        Appliance commands:
          add                       — Add a new appliance
          update <id>               — Update an appliance by ID
          delete <id>               — Delete an appliance by ID

        Category commands:
          categories                — List all categories
          add-category              — Add a new category
          update-category <id>      — Update a category by ID
          delete-category <id>      — Delete a category by ID

        Query commands:
          find all                  — Find all appliances
          find <category>           — Find appliances by category
          find all price=min;max    — Find appliances by price range
          cost <min> <max>          — Alias for find all price=min;max
          show <id>                 — Show appliance details by ID
          stats                     — Show warehouse statistics

        Mode commands:
          switch user               — Switch back to user mode

        Other commands:
          help                      — Show this help
          exit                      — Exit application
        """;

    public Task<CommandResult> ExecuteAsync(ParsedRequest request) =>
        Task.FromResult(CommandResult.Ok(HelpText));
}
