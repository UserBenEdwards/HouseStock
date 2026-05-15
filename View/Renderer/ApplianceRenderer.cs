using Domain.Entities;
using Service.DTOs;

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

    public static void RenderCategoryTable(IEnumerable<ApplianceCategory> categories)
    {
        var list = categories.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("  No categories found.");
            return;
        }

        Console.WriteLine("  " + new string('-', 55));
        Console.WriteLine($"  {"#",-5}| {"ID",-6}| {"Name",-25}| Description");
        Console.WriteLine("  " + new string('-', 55));

        for (int i = 0; i < list.Count; i++)
        {
            var c = list[i];
            Console.WriteLine(
                $"  {(i + 1).ToString(),-5}" +
                $"| {c.Id.ToString(),-6}" +
                $"| {Truncate(c.Name, 25),-25}" +
                $"| {Truncate(c.Description ?? "—", 30)}");
        }

        Console.WriteLine("  " + new string('-', 55));
        Console.WriteLine($"  Total: {list.Count} category(ies)");
    }

    public static void RenderStats(WarehouseStats stats)
    {
        const int w = 36;
        var sep = "  " + new string('-', w);
        Console.WriteLine(sep);
        Console.WriteLine($"  {"Appliances",-20}: {stats.TotalAppliances}");
        Console.WriteLine($"  {"Categories",-20}: {stats.TotalCategories}");
        Console.WriteLine(sep);
        Console.WriteLine($"  {"Min price",-20}: {(stats.MinPrice.HasValue ? $"${stats.MinPrice:N2}" : "—")}");
        Console.WriteLine($"  {"Max price",-20}: {(stats.MaxPrice.HasValue ? $"${stats.MaxPrice:N2}" : "—")}");
        Console.WriteLine($"  {"Avg price",-20}: {(stats.AvgPrice.HasValue  ? $"${stats.AvgPrice:N2}"  : "—")}");
        Console.WriteLine(sep);
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..(max - 3)] + "...";
}
