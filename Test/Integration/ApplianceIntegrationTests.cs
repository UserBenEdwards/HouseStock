using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Service.DTOs;
using Service.Interfaces;

namespace Test.Integration;

public class ApplianceIntegrationTests : IntegrationTestBase
{
    // ── Add → GetAll ───────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ThenGetAll_ReturnsAddedAppliance()
    {
        var provider = CreateProvider();
        var service = provider.GetRequiredService<IApplianceService>();

        await service.AddAsync(new AddApplianceRequest("Samsung TV", null, 500m, null));
        var result = (await service.GetAllAsync()).ToList();

        Assert.Single(result);
        Assert.Equal("Samsung TV", result[0].Name);
        Assert.Equal(500m, result[0].Price);
    }

    // ── Add → GetById ──────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ThenGetById_ReturnsCorrectAppliance()
    {
        var provider = CreateProvider();
        var service = provider.GetRequiredService<IApplianceService>();

        await service.AddAsync(new AddApplianceRequest("LG Fridge", "desc", 900m, null));
        var all = await service.GetAllAsync();
        var id = all.First().Id;

        var result = await service.GetByIdAsync(id);

        Assert.Equal("LG Fridge", result.Name);
    }

    // ── Duplicate ──────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_Duplicate_ThrowsDuplicateApplianceException()
    {
        var provider = CreateProvider();
        var service = provider.GetRequiredService<IApplianceService>();

        await service.AddAsync(new AddApplianceRequest("Dyson Vacuum", null, 300m, null));

        await Assert.ThrowsAsync<DuplicateApplianceException>(
            () => service.AddAsync(new AddApplianceRequest("Dyson Vacuum", null, 350m, null)));
    }

    // ── Update → GetById ───────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ThenGetById_ReturnsUpdatedValues()
    {
        var provider = CreateProvider();
        var service = provider.GetRequiredService<IApplianceService>();

        await service.AddAsync(new AddApplianceRequest("Old Name", null, 100m, null));
        var id = (await service.GetAllAsync()).First().Id;

        await service.UpdateAsync(new UpdateApplianceRequest(id, "New Name", null, 200m, null));
        var result = await service.GetByIdAsync(id);

        Assert.Equal("New Name", result.Name);
        Assert.Equal(200m, result.Price);
    }

    // ── Delete → GetById throws ────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ThenGetById_ThrowsApplianceNotFoundException()
    {
        var provider = CreateProvider();
        var service = provider.GetRequiredService<IApplianceService>();

        await service.AddAsync(new AddApplianceRequest("Bosch Washer", null, 700m, null));
        var id = (await service.GetAllAsync()).First().Id;

        await service.DeleteAsync(id);

        await Assert.ThrowsAsync<ApplianceNotFoundException>(
            () => service.GetByIdAsync(id));
    }

    // ── Filter by category ─────────────────────────────────────────────────

    [Fact]
    public async Task AddWithCategory_ThenGetByCategoryName_ReturnsOnlyMatching()
    {
        var provider = CreateProvider();
        var applianceService = provider.GetRequiredService<IApplianceService>();
        var categoryService = provider.GetRequiredService<ICategoryService>();

        await categoryService.AddAsync(new AddCategoryRequest("TVs", null));
        await categoryService.AddAsync(new AddCategoryRequest("Fridges", null));

        await applianceService.AddAsync(new AddApplianceRequest("Sony TV", null, 600m, "TVs"));
        await applianceService.AddAsync(new AddApplianceRequest("Samsung TV", null, 700m, "TVs"));
        await applianceService.AddAsync(new AddApplianceRequest("LG Fridge", null, 900m, "Fridges"));

        var tvs = (await applianceService.GetByCategoryNameAsync("TVs")).ToList();

        Assert.Equal(2, tvs.Count);
        Assert.All(tvs, a => Assert.Equal("TVs", a.Category?.Name));
    }

    // ── Filter by price range ──────────────────────────────────────────────

    [Fact]
    public async Task GetByPriceRangeAsync_ReturnsOnlyAppliancesInRange()
    {
        var provider = CreateProvider();
        var service = provider.GetRequiredService<IApplianceService>();

        await service.AddAsync(new AddApplianceRequest("Cheap",    null, 100m,  null));
        await service.AddAsync(new AddApplianceRequest("Mid",      null, 500m,  null));
        await service.AddAsync(new AddApplianceRequest("Expensive",null, 1500m, null));

        var result = (await service.GetByPriceRangeAsync(200m, 1000m)).ToList();

        Assert.Single(result);
        Assert.Equal("Mid", result[0].Name);
    }
}
