using System.ComponentModel.DataAnnotations;

namespace Psycology.Space.Dtos;

public record SlotDto(int Id, DateTime StartsAt, int DurationMinutes);
public record SlotDetailDto(int Id, DateTime StartsAt, int DurationMinutes, bool IsBooked);
public record CreateSlotRequest(
    [Required] DateTime StartsAt,
    [Range(15, 480)] int DurationMinutes = 60);
