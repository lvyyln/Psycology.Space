using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Psycology.Space.Data;
using Psycology.Space.Dtos;
using Psycology.Space.Services;

namespace Psycology.Space.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (
            RegisterRequest request,
            UserManager<ApplicationUser> userManager,
            TokenService tokenService) =>
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors.Select(e => e.Description));

            await userManager.AddToRoleAsync(user, "Client");

            var roles = await userManager.GetRolesAsync(user);
            var (token, expiresAt) = tokenService.CreateToken(user, roles);
            var userInfo = new UserInfo(user.FullName, user.Email!, roles.FirstOrDefault() ?? "Client");
            return Results.Ok(new AuthResponse(token, expiresAt, userInfo));
        });

        group.MapPost("/login", async (
            LoginRequest request,
            UserManager<ApplicationUser> userManager,
            TokenService tokenService) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
                return Results.Unauthorized();

            var roles = await userManager.GetRolesAsync(user);
            var (token, expiresAt) = tokenService.CreateToken(user, roles);
            var userInfo = new UserInfo(user.FullName, user.Email!, roles.FirstOrDefault() ?? "Client");
            return Results.Ok(new AuthResponse(token, expiresAt, userInfo));
        });

        group.MapGet("/me", (ClaimsPrincipal principal) =>
        {
            var fullName = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            var email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            return Results.Ok(new UserInfo(fullName, email, role));
        }).RequireAuthorization();
    }
}
