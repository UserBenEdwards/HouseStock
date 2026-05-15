using Domain.Entities;

namespace Domain.Specifications;

public class PriceRangeSpecification(decimal min, decimal max) : ISpecification<Appliance>
{
    public bool IsSatisfiedBy(Appliance entity) =>
        entity.Price >= min && entity.Price <= max;
}
