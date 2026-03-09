using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Psycology.Space.Data;
using Psycology.Space.Dtos;

namespace Psycology.Space.Endpoints;

public static class AppointmentEndpoints
{
    public static void MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/appointments").RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal principal, ApplicationDbContext db) =>
        {
            var isPsychologist = principal.IsInRole("Psychologist");
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var query = db.Appointments
                .Include(a => a.Slot)
                .Include(a => a.Client)
                .AsQueryable();

            if (!isPsychologist)
                query = query.Where(a => a.ClientId == userId);

            var appointments = await query
                .OrderBy(a => a.Slot.StartsAt)
                .Select(a => new AppointmentDto(
                    a.Id,
                    new SlotDto(a.Slot.Id, a.Slot.StartsAt, a.Slot.DurationMinutes),
                    a.Client.FullName,
                    a.Client.Email!,
                    a.Status,
                    a.Notes))
                .ToListAsync();

            return Results.Ok(appointments);
        });

        group.MapPost("/", async (
            BookAppointmentRequest request,
            ClaimsPrincipal principal,
            ApplicationDbContext db) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Atomic conditional update — prevents double-booking race condition
            var updated = await db.AvailableSlots
                .Where(s => s.Id == request.SlotId && !s.IsBooked)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsBooked, true));

            if (updated == 0)
                return Results.BadRequest("Slot is no longer available.");

            var appointment = new Appointment
            {
                SlotId = request.SlotId,
                ClientId = userId,
                Notes = request.Notes,
                Status = AppointmentStatus.Pending
            };

            db.Appointments.Add(appointment);
            await db.SaveChangesAsync();

            await db.Entry(appointment).Reference(a => a.Slot).LoadAsync();
            await db.Entry(appointment).Reference(a => a.Client).LoadAsync();

            return Results.Created($"/api/appointments/{appointment.Id}",
                new AppointmentDto(
                    appointment.Id,
                    new SlotDto(appointment.Slot.Id, appointment.Slot.StartsAt, appointment.Slot.DurationMinutes),
                    appointment.Client.FullName,
                    appointment.Client.Email!,
                    appointment.Status,
                    appointment.Notes));
        }).RequireAuthorization(p => p.RequireRole("Client"));

        group.MapDelete("/{id:int}", async (
            int id,
            ClaimsPrincipal principal,
            ApplicationDbContext db) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var appointment = await db.Appointments
                .Include(a => a.Slot)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment is null) return Results.NotFound();
            if (appointment.ClientId != userId) return Results.Forbid();
            if (appointment.Status == AppointmentStatus.Declined)
                return Results.BadRequest("Cannot cancel a declined appointment.");

            appointment.Slot.IsBooked = false;
            db.Appointments.Remove(appointment);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization(p => p.RequireRole("Client"));

        group.MapPut("/{id:int}/status", async (
            int id,
            UpdateStatusRequest request,
            ApplicationDbContext db) =>
        {
            var appointment = await db.Appointments.FindAsync(id);
            if (appointment is null) return Results.NotFound();

            appointment.Status = request.Status;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization(p => p.RequireRole("Psychologist"));
    }
}
