using Domain.Entities;
using Xunit;

namespace Test.TestData;

public class PriceRangeTestData : TheoryData<decimal, decimal, int>
{
    public PriceRangeTestData()
    {
        Add(100m,  500m, 2);
        Add(600m, 1000m, 1);
        Add(2000m, 5000m, 0);
        Add(300m,  800m, 3);
        Add(450m,  450m, 1);
    }

    public static IEnumerable<Appliance> SampleAppliances =>
    [
        new() { Id = 1, Name = "TV",     Price = 300m },
        new() { Id = 2, Name = "Fridge", Price = 450m },
        new() { Id = 3, Name = "Washer", Price = 800m },
    ];
}
