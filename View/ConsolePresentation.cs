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
                result = await HandleAddAsync();
            else if (input.StartsWith("update", StringComparison.OrdinalIgnoreCase) && session.IsAdmin)
                result = await HandleUpdateAsync(input);
            else if (input.Equals("switch admin", StringComparison.OrdinalIgnoreCase))
                result = await HandleSwitchAdminAsync();
            else
                result = await controller.ExecuteAsync(input);

            Render(result);

            if (result.ShouldExit)
                running = false;
        }
    }

    // ── Интерактивные обработчики ──────────────────────────────────────────

    private async Task<CommandResult> HandleAddAsync()
    {
        Console.WriteLine("\n  -- Add new appliance --");
        var name = Prompt("  Name");
        var description = Prompt("  Description (optional, Enter to skip)");
        var priceStr = Prompt("  Price");
        var category = Prompt("  Category (optional, Enter to skip)");

        return await controller.ExecuteWithArgsAsync("add", new Dictionary<string, string>
        {
            ["name"]        = name,
            ["description"] = string.IsNullOrWhiteSpace(description) ? "" : description,
            ["price"]       = priceStr,
            ["category"]    = string.IsNullOrWhiteSpace(category) ? "" : category
        });
    }

    private async Task<CommandResult> HandleUpdateAsync(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
            return CommandResult.Fail("Invalid ID. Usage: update <id>");

        // Сначала покажем текущие данные
        var currentResult = await controller.ExecuteAsync($"show {id}");
        if (!currentResult.Success)
            return currentResult;
        Render(currentResult);

        Console.WriteLine("\n  -- Update appliance (Enter to keep current value) --");
        var name = Prompt("  New name");
        var description = Prompt("  New description");
        var priceStr = Prompt("  New price");
        var category = Prompt("  New category");

        return await controller.ExecuteWithArgsAsync("update", new Dictionary<string, string>
        {
            ["id"]          = id.ToString(),
            ["name"]        = name,
            ["description"] = description,
            ["price"]       = priceStr,
            ["category"]    = category
        });
    }

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

    // ── Рендеринг результата ───────────────────────────────────────────────

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

        if (result.Data is IEnumerable<Appliance> list)
        {
            Console.WriteLine($"\n  {result.Message}");
            ApplianceRenderer.RenderTable(list);
        }
        else if (result.Data is Appliance appliance)
        {
            Console.WriteLine($"\n  {result.Message}");
            ApplianceRenderer.RenderDetail(appliance);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  {result.Message}");
            Console.ResetColor();
        }
    }

    // ── Вспомогательные методы ─────────────────────────────────────────────

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
