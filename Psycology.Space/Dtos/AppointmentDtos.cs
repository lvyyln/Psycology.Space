using System.ComponentModel.DataAnnotations;
using Psycology.Space.Data;

namespace Psycology.Space.Dtos;

public record AppointmentDto(int Id, SlotDto Slot, string ClientName, string ClientEmail, AppointmentStatus Status, string? Notes);
public record BookAppointmentRequest([Required] int SlotId, [MaxLength(500)] string? Notes);
public record UpdateStatusRequest(AppointmentStatus Status);
