using Domain.Entities;

namespace View.Renderer;

public static class ApplianceRenderer
{
    private const int ColId = 5;
    private const int ColName = 25;
    private const int ColCategory = 18;
    private const int ColPrice = 12;
    private const int ColDescription = 30;

    public static void RenderTable(IEnumerable<Appliance> appliances)
    {
        var list = appliances.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("  No appliances found.");
            return;
        }

        PrintSeparator();
        PrintRow("ID", "Name", "Category", "Price", "Description");
        PrintSeparator();

        foreach (var a in list)
            PrintRow(
                a.Id.ToString(),
                Truncate(a.Name, ColName),
                Truncate(a.Category?.Name ?? "—", ColCategory),
                $"${a.Price:N2}",
                Truncate(a.Description ?? "—", ColDescription)
            );

        PrintSeparator();
        Console.WriteLine($"  Total: {list.Count} item(s)");
    }

    public static void RenderDetail(Appliance appliance)
    {
        PrintSeparator();
        Console.WriteLine($"  ID          : {appliance.Id}");
        Console.WriteLine($"  Name        : {appliance.Name}");
        Console.WriteLine($"  Category    : {appliance.Category?.Name ?? "—"}");
        Console.WriteLine($"  Price       : ${appliance.Price:N2}");
        Console.WriteLine($"  Description : {appliance.Description ?? "—"}");
        Console.WriteLine($"  Created     : {appliance.CreatedAt:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"  Updated     : {appliance.UpdatedAt:yyyy-MM-dd HH:mm}");
        PrintSeparator();
    }

    private static void PrintRow(string id, string name, string category, string price, string description)
    {
        Console.WriteLine(
            $"  {id.PadRight(ColId)}" +
            $"| {name.PadRight(ColName)}" +
            $"| {category.PadRight(ColCategory)}" +
            $"| {price.PadRight(ColPrice)}" +
            $"| {description}");
    }

    private static void PrintSeparator()
    {
        var total = ColId + ColName + ColCategory + ColPrice + ColDescription + 12;
        Console.WriteLine("  " + new string('-', total));
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..(max - 3)] + "...";
}
