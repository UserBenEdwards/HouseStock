using AppController;
using AppController.Controllers;
using Domain.Entities;
using View.Renderer;

namespace View;

public class ConsolePresentation(
    QueryController controller,
    AppSession session)
{
    public async Task RunAsync()
    {
        PrintWelcome();

        var running = true;
        while (running)
        {
            var prompt = session.IsAdmin ? "[ADMIN]" : "[USER]";
            Console.Write($"\n{prompt} Enter command: ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input))
                continue;

            CommandResult result;

            // Команды требующие интерактивного ввода
            if (input.Equals("add", StringComparison.OrdinalIgnoreCase) && session.IsAdmin)
                result = await HandleAddApplianceAsync();
            else if (input.StartsWith("update ", StringComparison.OrdinalIgnoreCase) && session.IsAdmin)
                result = await HandleUpdateApplianceAsync(input);
            else if (input.Equals("add-category", StringComparison.OrdinalIgnoreCase) && session.IsAdmin)
                result = await HandleAddCategoryAsync();
            else if (input.StartsWith("update-category ", StringComparison.OrdinalIgnoreCase) && session.IsAdmin)
                result = await HandleUpdateCategoryAsync(input);
            else if (input.StartsWith("delete-category ", StringComparison.OrdinalIgnoreCase) && session.IsAdmin)
                result = await HandleDeleteCategoryAsync(input);
            else if (input.StartsWith("delete ", StringComparison.OrdinalIgnoreCase) && session.IsAdmin)
                result = await HandleDeleteApplianceAsync(input);
            else if (input.Equals("switch admin", StringComparison.OrdinalIgnoreCase))
                result = await HandleSwitchAdminAsync();
            else
                result = await controller.ExecuteAsync(input);

            Render(result);

            if (result.ShouldExit)
                running = false;
        }
    }

    // ── Appliance handlers ─────────────────────────────────────────────────

    private async Task<CommandResult> HandleAddApplianceAsync()
    {
        Console.WriteLine("\n  -- Add new appliance --");

        var name = Prompt("  Name");
        if (string.IsNullOrWhiteSpace(name))
            return CommandResult.Fail("Name cannot be empty.");

        var description = Prompt("  Description (optional, Enter to skip)");
        var priceStr = Prompt("  Price");

        var categoryName = await PickCategoryAsync() ?? string.Empty;

        return await controller.ExecuteWithArgsAsync("add", new Dictionary<string, string>
        {
            ["name"]        = name,
            ["description"] = description,
            ["price"]       = priceStr,
            ["category"]    = categoryName
        });
    }

    private async Task<CommandResult> HandleUpdateApplianceAsync(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
            return CommandResult.Fail("Invalid ID. Usage: update <id>");

        // Показываем текущие данные
        var currentResult = await controller.ExecuteAsync($"show {id}");
        if (!currentResult.Success)
            return currentResult;
        Render(currentResult);

        var current = currentResult.Data as Appliance;

        Console.WriteLine("\n  -- Update appliance (Enter to keep current value) --");

        var nameInput  = Prompt("  New name");
        var descInput  = Prompt("  New description");
        var priceInput = Prompt("  New price");
        var pickedCategory = await PickCategoryAsync(allowSkip: true);

        var name        = string.IsNullOrWhiteSpace(nameInput)  ? current?.Name        ?? string.Empty : nameInput;
        var description = string.IsNullOrWhiteSpace(descInput)  ? current?.Description ?? string.Empty : descInput;
        var priceStr    = string.IsNullOrWhiteSpace(priceInput) ?
            current?.Price.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? priceInput : priceInput;
        // null = keep existing; "" = explicit removal; "Name" = set new
        var category    = pickedCategory is null ? current?.Category?.Name ?? string.Empty : pickedCategory;

        return await controller.ExecuteWithArgsAsync("update", new Dictionary<string, string>
        {
            ["id"]          = id.ToString(),
            ["name"]        = name,
            ["description"] = description,
            ["price"]       = priceStr,
            ["category"]    = category
        });
    }

    // ── Category handlers ──────────────────────────────────────────────────

    private async Task<CommandResult> HandleAddCategoryAsync()
    {
        Console.WriteLine("\n  -- Add new category --");
        var name = Prompt("  Name");
        if (string.IsNullOrWhiteSpace(name))
            return CommandResult.Fail("Name cannot be empty.");

        var description = Prompt("  Description (optional, Enter to skip)");

        return await controller.ExecuteWithArgsAsync("add-category", new Dictionary<string, string>
        {
            ["name"]        = name,
            ["description"] = description
        });
    }

    private async Task<CommandResult> HandleUpdateCategoryAsync(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
            return CommandResult.Fail("Invalid ID. Usage: update-category <id>");

        var listResult = await controller.ExecuteAsync("categories");
        Render(listResult);

        var current = (listResult.Data as IEnumerable<ApplianceCategory>)
            ?.FirstOrDefault(c => c.Id == id);

        Console.WriteLine("\n  -- Update category (Enter to keep current value) --");
        var nameInput = Prompt("  New name");
        var descInput = Prompt("  New description");

        var name        = string.IsNullOrWhiteSpace(nameInput) ? current?.Name        ?? string.Empty : nameInput;
        var description = string.IsNullOrWhiteSpace(descInput) ? current?.Description ?? string.Empty : descInput;

        return await controller.ExecuteWithArgsAsync("update-category", new Dictionary<string, string>
        {
            ["id"]          = id.ToString(),
            ["name"]        = name,
            ["description"] = description
        });
    }

    private async Task<CommandResult> HandleDeleteApplianceAsync(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
            return CommandResult.Fail("Invalid ID. Usage: delete <id>");

        var showResult = await controller.ExecuteAsync($"show {id}");
        if (!showResult.Success)
            return showResult;

        var appliance = showResult.Data as Appliance;
        var displayName = appliance?.Name ?? $"#{id}";

        Console.Write($"\n  Are you sure you want to delete '{displayName}'? (y/N): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();
        if (confirm != "y")
            return CommandResult.Ok("Deletion cancelled.");

        return await controller.ExecuteWithArgsAsync("delete", new Dictionary<string, string>
        {
            ["id"] = id.ToString()
        });
    }

    private async Task<CommandResult> HandleDeleteCategoryAsync(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
            return CommandResult.Fail("Invalid ID. Usage: delete-category <id>");

        var listResult = await controller.ExecuteAsync("categories");
        var category = (listResult.Data as IEnumerable<ApplianceCategory>)
            ?.FirstOrDefault(c => c.Id == id);

        if (category is null)
            return CommandResult.Fail($"Category #{id} not found.");

        Console.Write($"\n  Are you sure you want to delete '{category.Name}'? (y/N): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();
        if (confirm != "y")
            return CommandResult.Ok("Deletion cancelled.");

        return await controller.ExecuteWithArgsAsync("delete-category", new Dictionary<string, string>
        {
            ["id"] = id.ToString()
        });
    }

    // ── Category picker ────────────────────────────────────────────────────

    // Returns: null = no selection made (keep existing), "" = explicit removal (0), "Name" = chosen
    private async Task<string?> PickCategoryAsync(bool allowSkip = false)
    {
        var categoriesResult = await controller.ExecuteAsync("categories");
        var categories = (categoriesResult.Data as IEnumerable<ApplianceCategory>)?.ToList()
                         ?? new List<ApplianceCategory>();

        if (categories.Count == 0)
        {
            Console.WriteLine("  (No categories available)");
            return null;
        }

        Console.WriteLine("\n  Available categories:");
        for (int i = 0; i < categories.Count; i++)
            Console.WriteLine($"    {i + 1}. {categories[i].Name}");

        var skipNote = allowSkip ? ", 0 to remove, Enter to keep" : ", 0 to skip";
        Console.Write($"  Select category (1-{categories.Count}{skipNote}): ");

        var input = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(input))
            return null;

        if (!int.TryParse(input, out var choice) || choice == 0)
            return string.Empty;

        if (choice < 1 || choice > categories.Count)
        {
            Console.WriteLine("  Invalid choice. No category will be assigned.");
            return string.Empty;
        }

        return categories[choice - 1].Name;
    }

    // ── Auth handler ───────────────────────────────────────────────────────

    private async Task<CommandResult> HandleSwitchAdminAsync()
    {
        Console.Write("  Password: ");
        var password = ReadMaskedInput();

        return await controller.ExecuteWithArgsAsync("switch", new Dictionary<string, string>
        {
            ["mode"]     = "admin",
            ["password"] = password
        });
    }

    // ── Renderer ───────────────────────────────────────────────────────────

    private static void Render(CommandResult result)
    {
        if (!result.Success)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  [ERROR] {result.Message}");
            Console.ResetColor();
            return;
        }

        if (result.ShouldExit)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n  {result.Message}");
            Console.ResetColor();
            return;
        }

        if (result.Data is IEnumerable<Appliance> applianceList)
        {
            Console.WriteLine($"\n  {result.Message}");
            ApplianceRenderer.RenderTable(applianceList);
        }
        else if (result.Data is Appliance appliance)
        {
            Console.WriteLine($"\n  {result.Message}");
            ApplianceRenderer.RenderDetail(appliance);
        }
        else if (result.Data is IEnumerable<ApplianceCategory> categories)
        {
            Console.WriteLine($"\n  {result.Message}");
            ApplianceRenderer.RenderCategoryTable(categories);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  {result.Message}");
            Console.ResetColor();
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static string Prompt(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static string ReadMaskedInput()
    {
        var password = string.Empty;
        ConsoleKeyInfo key;

        while ((key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
        {
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
            else if (key.Key != ConsoleKey.Backspace)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return password;
    }

    private static void PrintWelcome()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║   Welcome to Household Appliances Warehouse  ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine("  Type 'help' to see available commands.\n");
    }
}
