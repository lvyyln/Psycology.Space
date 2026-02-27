namespace Psycology.Space.Data;

public class Appointment
{
    public int Id { get; set; }
    public int SlotId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Notes { get; set; }

    public AvailableSlot Slot { get; set; } = null!;
    public ApplicationUser Client { get; set; } = null!;
}
