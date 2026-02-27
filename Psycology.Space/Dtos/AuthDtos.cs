namespace Psycology.Space.Dtos;

public record RegisterRequest(string FullName, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record UserInfo(string FullName, string Email, string Role);
public record AuthResponse(string Token, DateTime ExpiresAt, UserInfo User);
