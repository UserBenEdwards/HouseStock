namespace Domain.Exceptions;

public class DuplicateCategoryException : HousestockException
{
    public string Name { get; }

    public DuplicateCategoryException(string name)
        : base($"Category with name '{name}' already exists.")
    {
        Name = name;
    }
}
