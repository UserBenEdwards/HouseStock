namespace Domain.Entities;

public class Appliance
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    //Navigation Properties
    public ApplianceCategory? Category { get; set; }     
}