using Domain.Entities;
using Service.DTOs;

namespace Service.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<ApplianceCategory>> GetAllAsync();
    Task AddAsync(AddCategoryRequest request);
    Task UpdateAsync(UpdateCategoryRequest request);
    Task DeleteAsync(int id);
}
