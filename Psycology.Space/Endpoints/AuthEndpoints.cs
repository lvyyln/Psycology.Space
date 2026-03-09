using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            TokenService tokenService,
            HttpContext httpContext,
            IWebHostEnvironment env) =>
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
            var (accessToken, expiresAt) = tokenService.CreateAccessToken(user, roles);
            var refreshToken = await tokenService.CreateRefreshTokenAsync(user.Id);

            SetRefreshCookie(httpContext, refreshToken, env.IsProduction());

            var userInfo = new UserInfo(user.FullName, user.Email!, roles.FirstOrDefault() ?? "Client");
            return Results.Ok(new AuthResponse(accessToken, expiresAt, userInfo));
        });

        group.MapPost("/login", async (
            LoginRequest request,
            UserManager<ApplicationUser> userManager,
            TokenService tokenService,
            HttpContext httpContext,
            IWebHostEnvironment env) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
                return Results.Unauthorized();

            var roles = await userManager.GetRolesAsync(user);
            var (accessToken, expiresAt) = tokenService.CreateAccessToken(user, roles);
            var refreshToken = await tokenService.CreateRefreshTokenAsync(user.Id);

            SetRefreshCookie(httpContext, refreshToken, env.IsProduction());

            var userInfo = new UserInfo(user.FullName, user.Email!, roles.FirstOrDefault() ?? "Client");
            return Results.Ok(new AuthResponse(accessToken, expiresAt, userInfo));
        });

        group.MapPost("/refresh", async (
            HttpContext httpContext,
            ApplicationDbContext db,
            TokenService tokenService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env) =>
        {
            var rawToken = httpContext.Request.Cookies["refreshToken"];
            if (rawToken is null)
                return Results.Unauthorized();

            var stored = await db.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt =>
                    rt.Token == rawToken &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow);

            if (stored is null)
                return Results.Unauthorized();

            // Rotate: revoke old token, issue new one
            stored.IsRevoked = true;
            await db.SaveChangesAsync();
            var newRefreshToken = await tokenService.CreateRefreshTokenAsync(stored.UserId);

            var roles = await userManager.GetRolesAsync(stored.User);
            var (accessToken, expiresAt) = tokenService.CreateAccessToken(stored.User, roles);

            SetRefreshCookie(httpContext, newRefreshToken, env.IsProduction());

            var userInfo = new UserInfo(stored.User.FullName, stored.User.Email!, roles.FirstOrDefault() ?? "Client");
            return Results.Ok(new AuthResponse(accessToken, expiresAt, userInfo));
        });

        group.MapPost("/logout", async (
            HttpContext httpContext,
            ApplicationDbContext db) =>
        {
            var rawToken = httpContext.Request.Cookies["refreshToken"];
            if (rawToken is not null)
            {
                await db.RefreshTokens
                    .Where(rt => rt.Token == rawToken)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRevoked, true));
            }
            httpContext.Response.Cookies.Delete("refreshToken");
            return Results.NoContent();
        });

        group.MapGet("/me", (ClaimsPrincipal principal) =>
        {
            var fullName = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            var email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            return Results.Ok(new UserInfo(fullName, email, role));
        }).RequireAuthorization();
    }

    private static void SetRefreshCookie(HttpContext ctx, string token, bool secure) =>
        ctx.Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Secure = secure,
            Path = "/"
        });
}
