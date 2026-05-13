namespace Domain.Interfaces;

public interface IApplianceRepository
{
    Task<IEnumerable<Entities.Appliance>> GetAllAsync();
    Task<IEnumerable<Entities.Appliance>> GetByCategoryNameAsync(string categoryName);
    Task<IEnumerable<Entities.Appliance>> GetByPriceRangeAsync(decimal min, decimal max);
    Task<Entities.Appliance?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(string name);
    Task AddAsync(Entities.Appliance appliance);
    Task UpdateAsync(Entities.Appliance appliance);
    Task DeleteAsync(int id);
}
