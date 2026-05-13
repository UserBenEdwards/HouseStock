namespace Domain.Interfaces;

public interface IApplianceCategoryRepository
{
    Task<IEnumerable<Entities.ApplianceCategory>> GetAllAsync();
    Task<Entities.ApplianceCategory?> GetByIdAsync(int id);
    Task<Entities.ApplianceCategory?> GetByNameAsync(string name);
    Task<bool> ExistsAsync(string name);
    Task AddAsync(Entities.ApplianceCategory category);
}
