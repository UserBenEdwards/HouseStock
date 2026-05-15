namespace Domain.Exceptions;

public class DuplicateApplianceException : HousestockException
{
    public string Name { get; }

    public DuplicateApplianceException(string name)
        : base($"Appliance with name '{name}' already exists.")
    {
        Name = name;
    }
}
