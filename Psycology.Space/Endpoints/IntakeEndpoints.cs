using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Psycology.Space.Data;
using Psycology.Space.Dtos;

namespace Psycology.Space.Endpoints;

public static class IntakeEndpoints
{
    public static void MapIntakeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/intake").RequireAuthorization();

        group.MapPost("/", async (
            SubmitIntakeRequest request,
            ClaimsPrincipal principal,
            ApplicationDbContext db) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var response = new IntakeResponse
            {
                UserId = userId,
                MoodScore = request.MoodScore,
                SleepScore = request.SleepScore,
                AnxietyScore = request.AnxietyScore,
                MainConcern = request.MainConcern,
                AdditionalNotes = request.AdditionalNotes
            };

            db.IntakeResponses.Add(response);
            await db.SaveChangesAsync();

            return Results.Created($"/api/intake/{response.Id}", response.Id);
        }).RequireAuthorization(p => p.RequireRole("Client"));

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var responses = await db.IntakeResponses
                .Include(r => r.User)
                .OrderByDescending(r => r.SubmittedAt)
                .Select(r => new IntakeResponseDto(
                    r.Id,
                    r.User.FullName,
                    r.User.Email!,
                    r.SubmittedAt,
                    r.MoodScore,
                    r.SleepScore,
                    r.AnxietyScore,
                    r.MainConcern,
                    r.AdditionalNotes))
                .ToListAsync();

            return Results.Ok(responses);
        }).RequireAuthorization(p => p.RequireRole("Psychologist"));
    }
}
