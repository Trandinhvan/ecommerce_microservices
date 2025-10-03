namespace BasketService.Application.DTOs;

public record BasketItemDto(Guid ProductId, string ProductName, decimal Price, int Quantity);
public record BasketDto(List<BasketItemDto> Items);

public class UpdateItemDto
{
    public int Quantity { get; set; }
}
