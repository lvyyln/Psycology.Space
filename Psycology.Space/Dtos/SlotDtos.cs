namespace Psycology.Space.Dtos;

public record SlotDto(int Id, DateTime StartsAt, int DurationMinutes);
public record CreateSlotRequest(DateTime StartsAt, int DurationMinutes = 60);
