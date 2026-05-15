namespace Service.DTOs;

public record WarehouseStats(
    int TotalAppliances,
    int TotalCategories,
    decimal? MinPrice,
    decimal? MaxPrice,
    decimal? AvgPrice
);
