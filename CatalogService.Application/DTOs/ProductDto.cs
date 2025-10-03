public record ProductDto(Guid Id, string Name, string Description, decimal Price, Guid CategoryId);
public class CreateProductRequest
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
}

public record UpdateProductRequest(Guid Id, string Name, string Description, decimal Price, Guid CategoryId);

public record ProductDisplayDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    string CategoryName,
    string ImageUrl,
    DateTime CreatedAt
);

/// <summary>
/// Dùng cho trang chi tiết sản phẩm (detail page, cart)
/// </summary>
public record ProductDetailDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    decimal? OriginalPrice,
    int StockQuantity,
    string ImageUrl,
    string CategoryName,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
