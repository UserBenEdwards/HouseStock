using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;
using Service.DTOs;
using Service.Services;

namespace Test.Services;

public class ApplianceServiceTests
{
    private readonly Mock<IApplianceRepository> _repoMock = new();
    private readonly Mock<IApplianceCategoryRepository> _categoryRepoMock = new();

    private ApplianceService CreateService() =>
        new(_repoMock.Object, _categoryRepoMock.Object);

    // ── GetAllAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllAppliances()
    {
        var expected = new List<Appliance>
        {
            new() { Id = 1, Name = "TV", Price = 500 },
            new() { Id = 2, Name = "Fridge", Price = 900 }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await CreateService().GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Appliance>());

        var result = await CreateService().GetAllAsync();

        Assert.Empty(result);
    }

    // ── GetByIdAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsAppliance()
    {
        var appliance = new Appliance { Id = 1, Name = "TV", Price = 500 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appliance);

        var result = await CreateService().GetByIdAsync(1);

        Assert.Equal("TV", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ThrowsApplianceNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Appliance?)null);

        await Assert.ThrowsAsync<ApplianceNotFoundException>(
            () => CreateService().GetByIdAsync(99));
    }

    // ── GetByPriceRangeAsync ───────────────────────────────────────────────

    [Theory]
    [InlineData(100, 500, 2)]
    [InlineData(600, 1000, 1)]
    [InlineData(2000, 5000, 0)]
    public async Task GetByPriceRangeAsync_ReturnsFilteredAppliances(
        decimal min, decimal max, int expectedCount)
    {
        var appliances = new List<Appliance>
        {
            new() { Id = 1, Name = "TV",     Price = 300 },
            new() { Id = 2, Name = "Fridge", Price = 450 },
            new() { Id = 3, Name = "Washer", Price = 800 }
        };

        _repoMock
            .Setup(r => r.GetByPriceRangeAsync(min, max))
            .ReturnsAsync(appliances.Where(a => a.Price >= min && a.Price <= max).ToList());

        var result = await CreateService().GetByPriceRangeAsync(min, max);

        Assert.Equal(expectedCount, result.Count());
    }

    // ── AddAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_WhenValid_CallsRepositoryAdd()
    {
        _repoMock.Setup(r => r.ExistsAsync("TV")).ReturnsAsync(false);
        _categoryRepoMock.Setup(r => r.GetByNameAsync("Electronics"))
            .ReturnsAsync(new ApplianceCategory { Id = 1, Name = "Electronics" });

        await CreateService().AddAsync(new AddApplianceRequest("TV", "desc", 500, "Electronics"));

        _repoMock.Verify(r => r.AddAsync(It.IsAny<Appliance>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenDuplicate_ThrowsDuplicateApplianceException()
    {
        _repoMock.Setup(r => r.ExistsAsync("TV")).ReturnsAsync(true);

        await Assert.ThrowsAsync<DuplicateApplianceException>(
            () => CreateService().AddAsync(new AddApplianceRequest("TV", null, 500, null)));
    }

    [Fact]
    public async Task AddAsync_WhenCategoryNotFound_ThrowsCategoryNotFoundException()
    {
        _repoMock.Setup(r => r.ExistsAsync("TV")).ReturnsAsync(false);
        _categoryRepoMock.Setup(r => r.GetByNameAsync("Unknown"))
            .ReturnsAsync((ApplianceCategory?)null);

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => CreateService().AddAsync(new AddApplianceRequest("TV", null, 500, "Unknown")));
    }

    [Fact]
    public async Task AddAsync_WithoutCategory_DoesNotCallCategoryRepo()
    {
        _repoMock.Setup(r => r.ExistsAsync("TV")).ReturnsAsync(false);

        await CreateService().AddAsync(new AddApplianceRequest("TV", null, 500, null));

        _categoryRepoMock.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Appliance>()), Times.Once);
    }

    // ── UpdateAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenValid_CallsRepositoryUpdate()
    {
        var existing = new Appliance { Id = 1, Name = "TV", Price = 500 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.ExistsAsync("New TV")).ReturnsAsync(false);

        await CreateService().UpdateAsync(new UpdateApplianceRequest(1, "New TV", null, 600, null));

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Appliance>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ThrowsApplianceNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Appliance?)null);

        await Assert.ThrowsAsync<ApplianceNotFoundException>(
            () => CreateService().UpdateAsync(new UpdateApplianceRequest(99, "X", null, 1, null)));
    }

    [Fact]
    public async Task UpdateAsync_WhenSameName_DoesNotCheckDuplicate()
    {
        var existing = new Appliance { Id = 1, Name = "TV", Price = 500 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await CreateService().UpdateAsync(new UpdateApplianceRequest(1, "TV", null, 600, null));

        _repoMock.Verify(r => r.ExistsAsync(It.IsAny<string>()), Times.Never);
    }

    // ── DeleteAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenExists_CallsRepositoryDelete()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Appliance { Id = 1, Name = "TV" });

        await CreateService().DeleteAsync(1);

        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ThrowsApplianceNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Appliance?)null);

        await Assert.ThrowsAsync<ApplianceNotFoundException>(
            () => CreateService().DeleteAsync(99));
    }
}
