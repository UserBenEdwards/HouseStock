using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Запускаем только если таблица категорий пустая
        if (await context.ApplianceCategories.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var categories = new List<ApplianceCategory>
        {
            new() { Name = "Refrigerators",    Description = "Household and professional refrigerators",   CreatedAt = now, UpdatedAt = now },
            new() { Name = "Washing Machines", Description = "Automatic front-load and top-load washers",  CreatedAt = now, UpdatedAt = now },
            new() { Name = "Vacuum Cleaners",  Description = "Upright, robot, and wet-dry vacuums",        CreatedAt = now, UpdatedAt = now },
            new() { Name = "Televisions",      Description = "Smart TV, OLED, QLED and standard TVs",      CreatedAt = now, UpdatedAt = now },
            new() { Name = "Microwaves",       Description = "Solo, grill, and convection microwaves",     CreatedAt = now, UpdatedAt = now },
        };

        context.ApplianceCategories.AddRange(categories);
        await context.SaveChangesAsync();

        var appliances = new List<Appliance>
        {
            // Refrigerators
            new() { Name = "Samsung RB37A5200WW",   Description = "Double-door, NoFrost, 367 L",              Price = 549.99m,  Category = categories[0], CreatedAt = now, UpdatedAt = now },
            new() { Name = "LG GC-B459SMUM",        Description = "Double-door, Total NoFrost, 374 L",        Price = 620.00m,  Category = categories[0], CreatedAt = now, UpdatedAt = now },
            new() { Name = "Bosch KGV39XW22R",      Description = "Double-door, 354 L, silver finish",        Price = 480.50m,  Category = categories[0], CreatedAt = now, UpdatedAt = now },

            // Washing Machines
            new() { Name = "Haier HW70-BP1439",     Description = "7 kg, 1400 rpm, inverter motor",           Price = 310.00m,  Category = categories[1], CreatedAt = now, UpdatedAt = now },
            new() { Name = "LG F2J5HS6W",           Description = "7 kg, Steam wash, ThinQ smart control",    Price = 420.00m,  Category = categories[1], CreatedAt = now, UpdatedAt = now },
            new() { Name = "Bosch WGA142X0ME",      Description = "9 kg, 1200 rpm, EcoSilence Drive",         Price = 510.00m,  Category = categories[1], CreatedAt = now, UpdatedAt = now },

            // Vacuum Cleaners
            new() { Name = "Dyson V15 Detect",      Description = "Cordless, laser dust detection, 60 min",   Price = 699.99m,  Category = categories[2], CreatedAt = now, UpdatedAt = now },
            new() { Name = "Xiaomi Mi Robot S10+",  Description = "Robot vacuum, self-empty base, 3000 Pa",   Price = 350.00m,  Category = categories[2], CreatedAt = now, UpdatedAt = now },
            new() { Name = "Samsung VC18M3120VB",   Description = "Corded, 1800 W, bagged, HEPA filter",      Price = 120.00m,  Category = categories[2], CreatedAt = now, UpdatedAt = now },

            // Televisions
            new() { Name = "Samsung QE55Q80C",      Description = "55\", QLED, 4K, 120 Hz, HDR",              Price = 870.00m,  Category = categories[3], CreatedAt = now, UpdatedAt = now },
            new() { Name = "LG OLED55C3",           Description = "55\", OLED evo, 4K, 120 Hz, webOS",        Price = 1100.00m, Category = categories[3], CreatedAt = now, UpdatedAt = now },
            new() { Name = "Xiaomi TV A2 43",       Description = "43\", Full HD, Android TV, 60 Hz",         Price = 280.00m,  Category = categories[3], CreatedAt = now, UpdatedAt = now },

            // Microwaves
            new() { Name = "Samsung MS23K3513AW",   Description = "23 L, 800 W, solo, white",                 Price = 89.99m,   Category = categories[4], CreatedAt = now, UpdatedAt = now },
            new() { Name = "LG MH6042D",            Description = "20 L, 700 W, grill, auto-cook programs",   Price = 115.00m,  Category = categories[4], CreatedAt = now, UpdatedAt = now },
            new() { Name = "Bosch BEL554MS0",       Description = "25 L, 900 W, grill and convection",        Price = 210.00m,  Category = categories[4], CreatedAt = now, UpdatedAt = now },
        };

        context.Appliances.AddRange(appliances);
        await context.SaveChangesAsync();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  [Seed] Database seeded with initial data.");
        Console.ResetColor();
    }
}
