using Psycology.Space.Data;

namespace Psycology.Space.Dtos;

public record AppointmentDto(int Id, SlotDto Slot, string ClientName, AppointmentStatus Status, string? Notes);
public record BookAppointmentRequest(int SlotId, string? Notes);
public record UpdateStatusRequest(AppointmentStatus Status);
