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

            var slot = await db.AvailableSlots.FindAsync(request.SlotId);
            if (slot is null) return Results.NotFound("Slot not found.");
            if (slot.IsBooked) return Results.BadRequest("Slot is already booked.");

            slot.IsBooked = true;
            var appointment = new Appointment
            {
                SlotId = slot.Id,
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
                    appointment.Status,
                    appointment.Notes));
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
