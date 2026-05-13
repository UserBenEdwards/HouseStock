using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ApplianceRepository(AppDbContext context) : IApplianceRepository
{
    public async Task<IEnumerable<Appliance>> GetAllAsync()
    {
        return await context.Appliances
            .Include(a => a.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Appliance>> GetByCategoryNameAsync(string categoryName)
    {
        return await context.Appliances
            .Include(a => a.Category)
            .AsNoTracking()
            .Where(a => a.Category != null &&
                        a.Category.Name.ToLower() == categoryName.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Appliance>> GetByPriceRangeAsync(decimal min, decimal max)
    {
        return await context.Appliances
            .Include(a => a.Category)
            .AsNoTracking()
            .Where(a => a.Price >= min && a.Price <= max)
            .ToListAsync();
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
    }

    public async Task UpdateAsync(Appliance appliance)
    {
        appliance.UpdatedAt = DateTime.UtcNow;
        context.Appliances.Update(appliance);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var appliance = await context.Appliances.FindAsync(id);
        if (appliance is null) return;
        context.Appliances.Remove(appliance);
        await context.SaveChangesAsync();
    }
}
