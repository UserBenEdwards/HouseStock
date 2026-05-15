using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Service.DTOs;
using Service.Services;

namespace Test.Services;

public class CategoryServiceTests
{
    private readonly Mock<IApplianceCategoryRepository> _repoMock = new();
    private readonly Mock<ILogger<CategoryService>> _loggerMock = new();

    private CategoryService CreateService() =>
        new(_repoMock.Object, _loggerMock.Object);

    // ── GetAllAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        var expected = new List<ApplianceCategory>
        {
            new() { Id = 1, Name = "Refrigerators" },
            new() { Id = 2, Name = "Washing Machines" }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await CreateService().GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ApplianceCategory>());

        var result = await CreateService().GetAllAsync();

        Assert.Empty(result);
    }

    // ── AddAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_WhenValid_CallsRepositoryAdd()
    {
        _repoMock.Setup(r => r.ExistsAsync("Televisions")).ReturnsAsync(false);

        await CreateService().AddAsync(new AddCategoryRequest("Televisions", null));

        _repoMock.Verify(r => r.AddAsync(It.Is<ApplianceCategory>(c => c.Name == "Televisions")), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenDuplicate_ThrowsDuplicateCategoryException()
    {
        _repoMock.Setup(r => r.ExistsAsync("Televisions")).ReturnsAsync(true);

        await Assert.ThrowsAsync<DuplicateCategoryException>(
            () => CreateService().AddAsync(new AddCategoryRequest("Televisions", null)));
    }

    // ── UpdateAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenValid_CallsRepositoryUpdate()
    {
        var existing = new ApplianceCategory { Id = 1, Name = "TVs" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.ExistsAsync("Televisions")).ReturnsAsync(false);

        await CreateService().UpdateAsync(new UpdateCategoryRequest(1, "Televisions", null));

        _repoMock.Verify(r => r.UpdateAsync(It.Is<ApplianceCategory>(c => c.Name == "Televisions")), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ThrowsCategoryNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ApplianceCategory?)null);

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => CreateService().UpdateAsync(new UpdateCategoryRequest(99, "X", null)));
    }

    [Fact]
    public async Task UpdateAsync_WhenDuplicateName_ThrowsDuplicateCategoryException()
    {
        var existing = new ApplianceCategory { Id = 1, Name = "TVs" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.ExistsAsync("Televisions")).ReturnsAsync(true);

        await Assert.ThrowsAsync<DuplicateCategoryException>(
            () => CreateService().UpdateAsync(new UpdateCategoryRequest(1, "Televisions", null)));
    }

    [Fact]
    public async Task UpdateAsync_WhenSameName_DoesNotCheckDuplicate()
    {
        var existing = new ApplianceCategory { Id = 1, Name = "TVs" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await CreateService().UpdateAsync(new UpdateCategoryRequest(1, "TVs", "updated desc"));

        _repoMock.Verify(r => r.ExistsAsync(It.IsAny<string>()), Times.Never);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<ApplianceCategory>()), Times.Once);
    }

    // ── DeleteAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenValid_CallsRepositoryDelete()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ApplianceCategory { Id = 1, Name = "Refrigerators" });

        await CreateService().DeleteAsync(1);

        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ThrowsCategoryNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ApplianceCategory?)null);

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => CreateService().DeleteAsync(99));
    }
}
