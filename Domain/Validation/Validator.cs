using Domain.Exceptions;

namespace Domain.Validation;

public class Validator<T>
{
    private readonly T _target;
    private readonly List<string> _errors = [];

    private Validator(T target) => _target = target;

    public static Validator<T> For(T target) => new(target);

    public Validator<T> NotNullOrEmpty(Func<T, string?> selector, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(selector(_target)))
            _errors.Add($"{fieldName} must not be empty.");
        return this;
    }

    public Validator<T> MaxLength(Func<T, string?> selector, int max, string fieldName)
    {
        var val = selector(_target);
        if (val is not null && val.Length > max)
            _errors.Add($"{fieldName} must not exceed {max} characters.");
        return this;
    }

    public Validator<T> GreaterThan(Func<T, decimal> selector, decimal min, string fieldName)
    {
        if (selector(_target) <= min)
            _errors.Add($"{fieldName} must be greater than {min}.");
        return this;
    }

    public Validator<T> InRange(Func<T, decimal> selector, decimal min, decimal max, string fieldName)
    {
        var val = selector(_target);
        if (val < min || val > max)
            _errors.Add($"{fieldName} must be between {min} and {max}.");
        return this;
    }

    public void Validate()
    {
        if (_errors.Count > 0)
            throw new ValidationException(_errors);
    }
}
