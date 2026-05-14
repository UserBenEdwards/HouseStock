namespace Domain.Interfaces;

public interface IApplianceCategoryRepository
{
    Task<IEnumerable<Entities.ApplianceCategory>> GetAllAsync();
    Task<Entities.ApplianceCategory?> GetByIdAsync(int id);
    Task<Entities.ApplianceCategory?> GetByNameAsync(string name);
    Task<bool> ExistsAsync(string name);
    Task<bool> HasAppliancesAsync(int id);
    Task AddAsync(Entities.ApplianceCategory category);
    Task UpdateAsync(Entities.ApplianceCategory category);
    Task DeleteAsync(int id);
}
