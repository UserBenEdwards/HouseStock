using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class ApplianceService(
    IApplianceRepository applianceRepository,
    IApplianceCategoryRepository categoryRepository) : IApplianceService
{
    public async Task<IEnumerable<Appliance>> GetAllAsync()
    {
        return await applianceRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Appliance>> GetByCategoryNameAsync(string categoryName)
    {
        return await applianceRepository.GetByCategoryNameAsync(categoryName);
    }

    public async Task<IEnumerable<Appliance>> GetByPriceRangeAsync(decimal min, decimal max)
    {
        return await applianceRepository.GetByPriceRangeAsync(min, max);
    }

    public async Task<Appliance> GetByIdAsync(int id)
    {
        var appliance = await applianceRepository.GetByIdAsync(id);
        if (appliance is null)
            throw new ApplianceNotFoundException(id);

        return appliance;
    }

    public async Task AddAsync(AddApplianceRequest request)
    {
        if (await applianceRepository.ExistsAsync(request.Name))
            throw new DuplicateApplianceException(request.Name);

        int? categoryId = null;
        if (request.CategoryName is not null)
        {
            var category = await categoryRepository.GetByNameAsync(request.CategoryName);
            if (category is null)
                throw new CategoryNotFoundException(request.CategoryName);

            categoryId = category.Id;
        }

        var appliance = new Appliance
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = categoryId
        };

        await applianceRepository.AddAsync(appliance);
    }

    public async Task UpdateAsync(UpdateApplianceRequest request)
    {
        var appliance = await applianceRepository.GetByIdAsync(request.Id);
        if (appliance is null)
            throw new ApplianceNotFoundException(request.Id);

        // Проверяем дубликат только если имя изменилось
        if (!appliance.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)
            && await applianceRepository.ExistsAsync(request.Name))
        {
            throw new DuplicateApplianceException(request.Name);
        }

        int? categoryId = null;
        if (request.CategoryName is not null)
        {
            var category = await categoryRepository.GetByNameAsync(request.CategoryName);
            if (category is null)
                throw new CategoryNotFoundException(request.CategoryName);

            categoryId = category.Id;
        }

        appliance.Name = request.Name;
        appliance.Description = request.Description;
        appliance.Price = request.Price;
        appliance.CategoryId = categoryId;

        await applianceRepository.UpdateAsync(appliance);
    }

    public async Task DeleteAsync(int id)
    {
        var appliance = await applianceRepository.GetByIdAsync(id);
        if (appliance is null)
            throw new ApplianceNotFoundException(id);

        await applianceRepository.DeleteAsync(id);
    }
}
