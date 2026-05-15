using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class ApplianceRepository(
    AppDbContext context,
    ILogger<ApplianceRepository> logger) : IApplianceRepository
{
    public async Task<IEnumerable<Appliance>> GetAllAsync()
    {
        var result = await context.Appliances
            .Include(a => a.Category)
            .AsNoTracking()
            .ToListAsync();

        logger.LogDebug("GetAllAsync returned {Count} appliances", result.Count);
        return result;
    }

    public async Task<IEnumerable<Appliance>> GetByCategoryNameAsync(string categoryName)
    {
        var result = await context.Appliances
            .Include(a => a.Category)
            .AsNoTracking()
            .Where(a => a.Category != null &&
                        a.Category.Name.ToLower() == categoryName.ToLower())
            .ToListAsync();

        logger.LogDebug("GetByCategoryNameAsync('{Category}') returned {Count} appliances", categoryName, result.Count);
        return result;
    }

    public async Task<IEnumerable<Appliance>> GetByPriceRangeAsync(decimal min, decimal max)
    {
        var result = await context.Appliances
            .Include(a => a.Category)
            .AsNoTracking()
            .Where(a => a.Price >= min && a.Price <= max)
            .ToListAsync();

        logger.LogDebug("GetByPriceRangeAsync({Min}, {Max}) returned {Count} appliances", min, max, result.Count);
        return result;
    }

    public async Task<Appliance?> GetByIdAsync(int id)
    {
        return await context.Appliances
            .Include(a => a.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await context.Appliances
            .AnyAsync(a => a.Name.ToLower() == name.ToLower());
    }

    public async Task AddAsync(Appliance appliance)
    {
        appliance.CreatedAt = DateTime.UtcNow;
        appliance.UpdatedAt = DateTime.UtcNow;
        await context.Appliances.AddAsync(appliance);
        await context.SaveChangesAsync();
        logger.LogDebug("Appliance #{Id} '{Name}' saved to database", appliance.Id, appliance.Name);
    }

    public async Task UpdateAsync(Appliance appliance)
    {
        appliance.UpdatedAt = DateTime.UtcNow;
        context.Appliances.Update(appliance);
        await context.SaveChangesAsync();
        logger.LogDebug("Appliance #{Id} updated in database", appliance.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var appliance = await context.Appliances.FindAsync(id);
        if (appliance is null) return;
        context.Appliances.Remove(appliance);
        await context.SaveChangesAsync();
        logger.LogDebug("Appliance #{Id} removed from database", id);
    }
}
