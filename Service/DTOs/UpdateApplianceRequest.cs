namespace Service.DTOs;

public record UpdateApplianceRequest(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    string? CategoryName
);
