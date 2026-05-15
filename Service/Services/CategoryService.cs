using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class CategoryService(
    IApplianceCategoryRepository categoryRepository,
    ILogger<CategoryService> logger) : ICategoryService
{
    public async Task<IEnumerable<ApplianceCategory>> GetAllAsync() =>
        await categoryRepository.GetAllAsync();

    public async Task AddAsync(AddCategoryRequest request)
    {
        if (await categoryRepository.ExistsAsync(request.Name))
        {
            logger.LogWarning("Duplicate category name: '{Name}'", request.Name);
            throw new DuplicateApplianceException(request.Name);
        }

        await categoryRepository.AddAsync(new ApplianceCategory
        {
            Name = request.Name,
            Description = request.Description
        });

        logger.LogInformation("Category '{Name}' created", request.Name);
    }

    public async Task UpdateAsync(UpdateCategoryRequest request)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
        {
            logger.LogWarning("Category #{Id} not found for update", request.Id);
            throw new CategoryNotFoundException(request.Id);
        }

        if (!category.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)
            && await categoryRepository.ExistsAsync(request.Name))
        {
            logger.LogWarning("Duplicate category name on update: '{Name}'", request.Name);
            throw new DuplicateApplianceException(request.Name);
        }

        category.Name = request.Name;
        category.Description = request.Description;

        await categoryRepository.UpdateAsync(category);
        logger.LogInformation("Category #{Id} updated", category.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            logger.LogWarning("Category #{Id} not found for deletion", id);
            throw new CategoryNotFoundException(id);
        }

        await categoryRepository.DeleteAsync(id);
        logger.LogInformation("Category #{Id} deleted", id);
    }
}
