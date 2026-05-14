namespace Service.DTOs;

public record AddApplianceRequest(
    string Name,
    string? Description,
    decimal Price,
    string? CategoryName
);
