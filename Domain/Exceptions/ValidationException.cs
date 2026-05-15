namespace Domain.Exceptions;

public class ValidationException : HousestockException
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors)
        : base($"Validation failed: {string.Join("; ", errors)}")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
