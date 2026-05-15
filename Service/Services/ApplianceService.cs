using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Specifications;
using Microsoft.Extensions.Logging;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class ApplianceService(
    IApplianceRepository applianceRepository,
    IApplianceCategoryRepository categoryRepository,
    ILogger<ApplianceService> logger) : IApplianceService
{
    public async Task<IEnumerable<Appliance>> GetAllAsync()
    {
        return await applianceRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Appliance>> GetByCategoryNameAsync(string categoryName)
    {
        var all = await applianceRepository.GetAllAsync();
        var spec = new ByCategorySpecification(categoryName);
        return all.Where(spec.IsSatisfiedBy);
    }

    public async Task<IEnumerable<Appliance>> GetByPriceRangeAsync(decimal min, decimal max)
    {
        var all = await applianceRepository.GetAllAsync();
        var spec = new PriceRangeSpecification(min, max);
        return all.Where(spec.IsSatisfiedBy);
    }

    public async Task<Appliance> GetByIdAsync(int id)
    {
        var appliance = await applianceRepository.GetByIdAsync(id);
        if (appliance is null)
        {
            logger.LogWarning("Appliance #{Id} not found", id);
            throw new ApplianceNotFoundException(id);
        }

        return appliance;
    }

    public async Task AddAsync(AddApplianceRequest request)
    {
        if (await applianceRepository.ExistsAsync(request.Name))
        {
            logger.LogWarning("Duplicate appliance name: '{Name}'", request.Name);
            throw new DuplicateApplianceException(request.Name);
        }

        int? categoryId = null;
        if (request.CategoryName is not null)
        {
            var category = await categoryRepository.GetByNameAsync(request.CategoryName);
            if (category is null)
            {
                logger.LogWarning("Category '{Name}' not found while adding appliance", request.CategoryName);
                throw new CategoryNotFoundException(request.CategoryName);
            }

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
        logger.LogInformation("Appliance '{Name}' added (price: {Price})", appliance.Name, appliance.Price);
    }

    public async Task UpdateAsync(UpdateApplianceRequest request)
    {
        var appliance = await applianceRepository.GetByIdAsync(request.Id);
        if (appliance is null)
        {
            logger.LogWarning("Appliance #{Id} not found for update", request.Id);
            throw new ApplianceNotFoundException(request.Id);
        }

        if (!appliance.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)
            && await applianceRepository.ExistsAsync(request.Name))
        {
            logger.LogWarning("Duplicate appliance name on update: '{Name}'", request.Name);
            throw new DuplicateApplianceException(request.Name);
        }

        int? categoryId = null;
        if (request.CategoryName is not null)
        {
            var category = await categoryRepository.GetByNameAsync(request.CategoryName);
            if (category is null)
            {
                logger.LogWarning("Category '{Name}' not found while updating appliance", request.CategoryName);
                throw new CategoryNotFoundException(request.CategoryName);
            }

            categoryId = category.Id;
        }

        appliance.Name = request.Name;
        appliance.Description = request.Description;
        appliance.Price = request.Price;
        appliance.CategoryId = categoryId;

        await applianceRepository.UpdateAsync(appliance);
        logger.LogInformation("Appliance #{Id} updated", appliance.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var appliance = await applianceRepository.GetByIdAsync(id);
        if (appliance is null)
        {
            logger.LogWarning("Appliance #{Id} not found for deletion", id);
            throw new ApplianceNotFoundException(id);
        }

        await applianceRepository.DeleteAsync(id);
        logger.LogInformation("Appliance #{Id} deleted", id);
    }
}
