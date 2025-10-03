namespace PaymentService.Application.DTOs;

//public record PaymentRequest(Guid OrderId, decimal Amount, string UserId);
//public record PaymentResponse(Guid PaymentId, Guid OrderId, string Status);
public record PaymentDto(Guid Id, Guid OrderId, string UserId, decimal Amount, string Status);
