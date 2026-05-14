using Domain.Entities;
using Service.DTOs;

namespace Service.Interfaces;

public interface IApplianceService
{
    Task<IEnumerable<Appliance>> GetAllAsync();
    Task<IEnumerable<Appliance>> GetByCategoryNameAsync(string categoryName);
    Task<IEnumerable<Appliance>> GetByPriceRangeAsync(decimal min, decimal max);
    Task<Appliance> GetByIdAsync(int id);
    Task AddAsync(AddApplianceRequest request);
    Task UpdateAsync(UpdateApplianceRequest request);
    Task DeleteAsync(int id);
}
