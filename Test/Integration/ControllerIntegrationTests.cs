using AppController;
using AppController.Controllers;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Service.DTOs;
using Service.Interfaces;

namespace Test.Integration;

// Тесты цепочки Controller → Service → Repository
public class ControllerIntegrationTests : IntegrationTestBase
{
    // ── find all ───────────────────────────────────────────────────────────

    [Fact]
    public async Task FindAll_WhenEmpty_ReturnsSuccessWithEmptyList()
    {
        var provider = CreateProvider();
        var controller = provider.GetRequiredService<QueryController>();

        var result = await controller.ExecuteAsync("find all");

        Assert.True(result.Success);
        var data = Assert.IsAssignableFrom<IEnumerable<Appliance>>(result.Data);
        Assert.Empty(data);
    }

    [Fact]
    public async Task FindAll_AfterAdd_ReturnsAppliance()
    {
        var provider = CreateProvider();
        var controller = provider.GetRequiredService<QueryController>();
        var service = provider.GetRequiredService<IApplianceService>();

        await service.AddAsync(new AddApplianceRequest("Philips TV", null, 450m, null));

        var result = await controller.ExecuteAsync("find all");

        Assert.True(result.Success);
        var data = Assert.IsAssignableFrom<IEnumerable<Appliance>>(result.Data);
        Assert.Single(data);
    }

    // ── show ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Show_ExistingId_ReturnsAppliance()
    {
        var provider = CreateProvider();
        var controller = provider.GetRequiredService<QueryController>();
        var service = provider.GetRequiredService<IApplianceService>();

        await service.AddAsync(new AddApplianceRequest("Bosch Oven", null, 800m, null));
        var id = (await service.GetAllAsync()).First().Id;

        var result = await controller.ExecuteAsync($"show {id}");

        Assert.True(result.Success);
        var appliance = Assert.IsType<Appliance>(result.Data);
        Assert.Equal("Bosch Oven", appliance.Name);
    }

    [Fact]
    public async Task Show_NonExistingId_ReturnsFailure()
    {
        var provider = CreateProvider();
        var controller = provider.GetRequiredService<QueryController>();

        var result = await controller.ExecuteAsync("show 9999");

        Assert.False(result.Success);
    }

    // ── find by category ───────────────────────────────────────────────────

    [Fact]
    public async Task FindByCategory_ReturnsOnlyMatchingAppliances()
    {
        var provider = CreateProvider();
        var controller = provider.GetRequiredService<QueryController>();
        var applianceService = provider.GetRequiredService<IApplianceService>();
        var categoryService = provider.GetRequiredService<ICategoryService>();

        await categoryService.AddAsync(new AddCategoryRequest("Laptops", null));
        await applianceService.AddAsync(new AddApplianceRequest("Dell XPS", null, 1200m, "Laptops"));
        await applianceService.AddAsync(new AddApplianceRequest("LG Fridge", null, 900m, null));

        var result = await controller.ExecuteAsync("find Laptops");

        Assert.True(result.Success);
        var data = Assert.IsAssignableFrom<IEnumerable<Appliance>>(result.Data);
        Assert.Single(data);
        Assert.Equal("Dell XPS", data.First().Name);
    }

    // ── cost (price range) ─────────────────────────────────────────────────

    [Fact]
    public async Task Cost_ReturnsAppliancesInPriceRange()
    {
        var provider = CreateProvider();
        var controller = provider.GetRequiredService<QueryController>();
        var service = provider.GetRequiredService<IApplianceService>();

        await service.AddAsync(new AddApplianceRequest("Budget", null, 150m, null));
        await service.AddAsync(new AddApplianceRequest("Premium", null, 2000m, null));

        var result = await controller.ExecuteAsync("cost 100 500");

        Assert.True(result.Success);
        var data = Assert.IsAssignableFrom<IEnumerable<Appliance>>(result.Data);
        Assert.Single(data);
        Assert.Equal("Budget", data.First().Name);
    }

    // ── categories ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Categories_AfterAdd_ReturnsCategory()
    {
        var provider = CreateProvider();
        var controller = provider.GetRequiredService<QueryController>();
        var categoryService = provider.GetRequiredService<ICategoryService>();

        await categoryService.AddAsync(new AddCategoryRequest("Microwaves", null));

        var result = await controller.ExecuteAsync("categories");

        Assert.True(result.Success);
        var data = Assert.IsAssignableFrom<IEnumerable<ApplianceCategory>>(result.Data);
        Assert.Single(data);
        Assert.Equal("Microwaves", data.First().Name);
    }

    // ── switch to admin ────────────────────────────────────────────────────

    [Fact]
    public async Task SwitchAdmin_WithCorrectPassword_SetsAdminSession()
    {
        var provider = CreateProvider();
        var controller = provider.GetRequiredService<QueryController>();
        var session = provider.GetRequiredService<AppSession>();

        var result = await controller.ExecuteWithArgsAsync("switch", new Dictionary<string, string>
        {
            ["mode"]     = "admin",
            ["password"] = "admin123"
        });

        Assert.True(result.Success);
        Assert.True(session.IsAdmin);
    }

    [Fact]
    public async Task SwitchAdmin_WithWrongPassword_ReturnsFail()
    {
        var provider = CreateProvider();
        var controller = provider.GetRequiredService<QueryController>();
        var session = provider.GetRequiredService<AppSession>();

        var result = await controller.ExecuteWithArgsAsync("switch", new Dictionary<string, string>
        {
            ["mode"]     = "admin",
            ["password"] = "wrong"
        });

        Assert.False(result.Success);
        Assert.False(session.IsAdmin);
    }
}
