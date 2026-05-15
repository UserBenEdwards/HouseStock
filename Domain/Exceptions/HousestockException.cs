namespace Domain.Exceptions;

public abstract class HousestockException : Exception
{
    protected HousestockException(string message) : base(message) { }
    protected HousestockException(string message, Exception inner) : base(message, inner) { }
}
