namespace Domain.Exceptions;

public class CategoryHasAppliancesException : Exception
{
    public CategoryHasAppliancesException(string name)
        : base($"Category '{name}' cannot be deleted because it has appliances assigned to it. Reassign them first.") { }
}
