using System.ComponentModel.DataAnnotations;

namespace Psycology.Space.Dtos;

public record RegisterRequest(
    [Required, MinLength(2), MaxLength(100)] string FullName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
public record UserInfo(string FullName, string Email, string Role);
public record AuthResponse(string Token, DateTime ExpiresAt, UserInfo User);

public record SubmitIntakeRequest(
    [Range(1, 5)] int MoodScore,
    [Range(1, 5)] int SleepScore,
    [Range(1, 5)] int AnxietyScore,
    [MaxLength(500)] string? MainConcern,
    [MaxLength(1000)] string? AdditionalNotes);

public record IntakeResponseDto(
    int Id, string ClientName, string ClientEmail,
    DateTime SubmittedAt,
    int MoodScore, int SleepScore, int AnxietyScore,
    string? MainConcern, string? AdditionalNotes);
