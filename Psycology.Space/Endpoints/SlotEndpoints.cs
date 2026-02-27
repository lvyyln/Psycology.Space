using Microsoft.EntityFrameworkCore;
using Psycology.Space.Data;
using Psycology.Space.Dtos;

namespace Psycology.Space.Endpoints;

public static class SlotEndpoints
{
    public static void MapSlotEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/slots");

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var slots = await db.AvailableSlots
                .Where(s => !s.IsBooked)
                .OrderBy(s => s.StartsAt)
                .Select(s => new SlotDto(s.Id, s.StartsAt, s.DurationMinutes))
                .ToListAsync();
            return Results.Ok(slots);
        });

        group.MapPost("/", async (CreateSlotRequest request, ApplicationDbContext db) =>
        {
            var slot = new AvailableSlot
            {
                StartsAt = request.StartsAt,
                DurationMinutes = request.DurationMinutes
            };
            db.AvailableSlots.Add(slot);
            await db.SaveChangesAsync();
            return Results.Created($"/api/slots/{slot.Id}", new SlotDto(slot.Id, slot.StartsAt, slot.DurationMinutes));
        }).RequireAuthorization(p => p.RequireRole("Psychologist"));

        group.MapDelete("/{id:int}", async (int id, ApplicationDbContext db) =>
        {
            var slot = await db.AvailableSlots.FindAsync(id);
            if (slot is null) return Results.NotFound();
            if (slot.IsBooked) return Results.BadRequest("Cannot delete a booked slot.");

            db.AvailableSlots.Remove(slot);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization(p => p.RequireRole("Psychologist"));
    }
}
