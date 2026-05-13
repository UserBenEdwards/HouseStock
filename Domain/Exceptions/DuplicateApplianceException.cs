namespace Domain.Exceptions;

public class DuplicateApplianceException : Exception
{
    public string Name { get; }

    public DuplicateApplianceException(string name)
        : base($"Appliance with name '{name}' already exists.")
    {
        Name = name;
    }
}
