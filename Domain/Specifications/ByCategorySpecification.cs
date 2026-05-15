using Domain.Entities;

namespace Domain.Specifications;

public class ByCategorySpecification(string categoryName) : ISpecification<Appliance>
{
    public bool IsSatisfiedBy(Appliance entity) =>
        entity.Category?.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase) == true;
}
