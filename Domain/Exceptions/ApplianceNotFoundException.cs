namespace Domain.Exceptions;

public class ApplianceNotFoundException : HousestockException
{
    public int Id { get; }

    public ApplianceNotFoundException(int id)
        : base($"Appliance with ID {id} was not found.")
    {
        Id = id;
    }
}
