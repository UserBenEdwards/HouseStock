namespace Domain.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(string name)
        : base($"Category '{name}' was not found.") { }

    public CategoryNotFoundException(int id)
        : base($"Category with ID {id} was not found.") { }
}
