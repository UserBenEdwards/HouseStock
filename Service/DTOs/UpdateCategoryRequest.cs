namespace Service.DTOs;

public record UpdateCategoryRequest(int Id, string Name, string? Description);
