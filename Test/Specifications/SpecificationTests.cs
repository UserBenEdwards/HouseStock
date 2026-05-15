using Domain.Entities;
using Domain.Specifications;
using Test.TestData;
using Xunit;

namespace Test.Specifications;

public class SpecificationTests
{
    private static Appliance MakeAppliance(decimal price, string? categoryName = null) => new()
    {
        Id = 1,
        Name = "Test",
        Price = price,
        Category = categoryName is null ? null : new ApplianceCategory { Name = categoryName }
    };

    // ── PriceRangeSpecification — MemberData ──────────────────────────────

    public static IEnumerable<object[]> PriceRangeMemberData =>
    [
        [100.0, 500.0, 300.0,  true],
        [100.0, 500.0, 600.0,  false],
        [300.0, 300.0, 300.0,  true],
        [0.0,   0.0,   1.0,    false],
        [500.0, 100.0, 300.0,  false],   // inverted range — nothing matches
    ];

    [Theory]
    [MemberData(nameof(PriceRangeMemberData))]
    public void PriceRangeSpec_IsSatisfiedBy(double min, double max, double price, bool expected)
    {
        var spec = new PriceRangeSpecification((decimal)min, (decimal)max);
        Assert.Equal(expected, spec.IsSatisfiedBy(MakeAppliance((decimal)price)));
    }

    // ── PriceRangeSpecification — ClassData ───────────────────────────────

    [Theory]
    [ClassData(typeof(PriceRangeTestData))]
    public void PriceRangeSpec_WithClassData_FiltersCorrectly(decimal min, decimal max, int expectedCount)
    {
        var spec = new PriceRangeSpecification(min, max);
        var result = PriceRangeTestData.SampleAppliances.Where(spec.IsSatisfiedBy).ToList();
        Assert.Equal(expectedCount, result.Count);
    }

    // ── ByCategorySpecification — MemberData ──────────────────────────────

    public static IEnumerable<object[]> CategoryMemberData =>
    [
        ["Electronics", "Electronics", true],
        ["Electronics", "ELECTRONICS", true],   // case-insensitive
        ["Electronics", "Kitchen",     false],
    ];

    [Theory]
    [MemberData(nameof(CategoryMemberData))]
    public void ByCategorySpec_IsSatisfiedBy(string filterName, string applCategoryName, bool expected)
    {
        var spec = new ByCategorySpecification(filterName);
        Assert.Equal(expected, spec.IsSatisfiedBy(MakeAppliance(100m, applCategoryName)));
    }

    [Fact]
    public void ByCategorySpec_WhenNoCategory_ReturnsFalse()
    {
        var spec = new ByCategorySpecification("Electronics");
        Assert.False(spec.IsSatisfiedBy(MakeAppliance(100m, null)));
    }

    // ── AndSpecification ──────────────────────────────────────────────────

    public static IEnumerable<object[]> AndSpecData =>
    [
        [200.0, "Electronics", true],
        [200.0, "Kitchen",     false],
        [900.0, "Electronics", false],
        [900.0, "Kitchen",     false],
    ];

    [Theory]
    [MemberData(nameof(AndSpecData))]
    public void AndSpec_CombinesPriceAndCategory(double price, string categoryName, bool expected)
    {
        var priceSpec    = new PriceRangeSpecification(100m, 500m);
        var categorySpec = new ByCategorySpecification("Electronics");
        var andSpec      = priceSpec.And(categorySpec);

        Assert.Equal(expected, andSpec.IsSatisfiedBy(MakeAppliance((decimal)price, categoryName)));
    }

    // ── OrSpecification ───────────────────────────────────────────────────

    public static IEnumerable<object[]> OrSpecData =>
    [
        [200.0, "Electronics", true],
        [200.0, "Kitchen",     true],
        [900.0, "Electronics", true],
        [900.0, "Kitchen",     false],
    ];

    [Theory]
    [MemberData(nameof(OrSpecData))]
    public void OrSpec_CombinesPriceAndCategory(double price, string categoryName, bool expected)
    {
        var priceSpec    = new PriceRangeSpecification(100m, 500m);
        var categorySpec = new ByCategorySpecification("Electronics");
        var orSpec       = priceSpec.Or(categorySpec);

        Assert.Equal(expected, orSpec.IsSatisfiedBy(MakeAppliance((decimal)price, categoryName)));
    }
}
