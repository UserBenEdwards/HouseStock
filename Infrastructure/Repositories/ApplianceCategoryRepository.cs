using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class ApplianceCategoryRepository(
    AppDbContext context,
    ILogger<ApplianceCategoryRepository> logger) : IApplianceCategoryRepository
{
    public async Task<IEnumerable<ApplianceCategory>> GetAllAsync()
    {
        var result = await context.ApplianceCategories
            .AsNoTracking()
            .ToListAsync();

        logger.LogDebug("GetAllAsync returned {Count} categories", result.Count);
        return result;
    }

    public async Task<ApplianceCategory?> GetByIdAsync(int id)
    {
        return await context.ApplianceCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ApplianceCategory?> GetByNameAsync(string name)
    {
        return await context.ApplianceCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await context.ApplianceCategories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> HasAppliancesAsync(int id)
    {
        return await context.Appliances.AnyAsync(a => a.CategoryId == id);
    }

    public async Task AddAsync(ApplianceCategory category)
    {
        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;
        await context.ApplianceCategories.AddAsync(category);
        await context.SaveChangesAsync();
        logger.LogDebug("Category #{Id} '{Name}' saved to database", category.Id, category.Name);
    }

    public async Task UpdateAsync(ApplianceCategory category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        var tracked = await context.ApplianceCategories.FindAsync(category.Id);
        if (tracked is null) return;
        context.Entry(tracked).CurrentValues.SetValues(category);
        await context.SaveChangesAsync();
        logger.LogDebug("Category #{Id} updated in database", category.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await context.ApplianceCategories.FindAsync(id);
        if (category is null) return;
        context.ApplianceCategories.Remove(category);
        await context.SaveChangesAsync();
        logger.LogDebug("Category #{Id} removed from database", id);
    }
}
