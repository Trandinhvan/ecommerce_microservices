namespace IdentityService.Application.DTOs;

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Username, string Password);
public record AuthResponse(string AccessToken, string RefreshToken);
