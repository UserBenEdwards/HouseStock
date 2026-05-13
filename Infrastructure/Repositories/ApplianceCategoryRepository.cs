using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ApplianceCategoryRepository(AppDbContext context) : IApplianceCategoryRepository
{
    public async Task<IEnumerable<ApplianceCategory>> GetAllAsync()
    {
        return await context.ApplianceCategories
            .AsNoTracking()
            .ToListAsync();
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

    public async Task AddAsync(ApplianceCategory category)
    {
        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;
        await context.ApplianceCategories.AddAsync(category);
        await context.SaveChangesAsync();
    }
}
