namespace Psycology.Space.Data;

public class AvailableSlot
{
    public int Id { get; set; }
    public DateTime StartsAt { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public bool IsBooked { get; set; }
}
