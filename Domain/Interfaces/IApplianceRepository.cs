namespace Domain.Interfaces;

public interface IApplianceRepository
{
    Task<IEnumerable<Entities.Appliance>> GetAllAsync();
    Task<Entities.Appliance?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(string name);
    Task AddAsync(Entities.Appliance appliance);
    Task UpdateAsync(Entities.Appliance appliance);
    Task DeleteAsync(int id);
}
