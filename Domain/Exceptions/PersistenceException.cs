namespace Domain.Exceptions;

public class PersistenceException : HousestockException
{
    public PersistenceException(string message, Exception inner)
        : base(message, inner) { }
}
