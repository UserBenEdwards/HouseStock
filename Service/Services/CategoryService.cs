using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class CategoryService(IApplianceCategoryRepository categoryRepository) : ICategoryService
{
    public async Task<IEnumerable<ApplianceCategory>> GetAllAsync() =>
        await categoryRepository.GetAllAsync();

    public async Task AddAsync(AddCategoryRequest request)
    {
        if (await categoryRepository.ExistsAsync(request.Name))
            throw new DuplicateApplianceException(request.Name);

        await categoryRepository.AddAsync(new ApplianceCategory
        {
            Name = request.Name,
            Description = request.Description
        });
    }

    public async Task UpdateAsync(UpdateCategoryRequest request)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
            throw new CategoryNotFoundException(request.Id);

        if (!category.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)
            && await categoryRepository.ExistsAsync(request.Name))
            throw new DuplicateApplianceException(request.Name);

        category.Name = request.Name;
        category.Description = request.Description;

        await categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
            throw new CategoryNotFoundException(id);

        await categoryRepository.DeleteAsync(id);
    }
}
