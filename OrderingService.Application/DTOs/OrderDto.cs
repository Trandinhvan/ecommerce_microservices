namespace OrderingService.Application.DTOs;

public record OrderItemDto(Guid ProductId, string ProductName, decimal Price, int Quantity);
public record CreateOrderRequest(List<OrderItemDto> Items);
public record OrderDto(Guid Id, string UserId, List<OrderItemDto> Items, decimal TotalPrice, string Status);
