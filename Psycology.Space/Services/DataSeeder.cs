using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Psycology.Space.Data;

namespace Psycology.Space.Services;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync("Psychologist"))
            await roleManager.CreateAsync(new IdentityRole("Psychologist"));

        if (!await roleManager.RoleExistsAsync("Client"))
            await roleManager.CreateAsync(new IdentityRole("Client"));

        const string psychEmail = "psych@psycology.space";
        if (await userManager.FindByEmailAsync(psychEmail) is null)
        {
            var psychologist = new ApplicationUser
            {
                UserName = psychEmail,
                Email = psychEmail,
                FullName = "Dr. Psychologist",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(psychologist, "Psychologist1!");
            await userManager.AddToRoleAsync(psychologist, "Psychologist");
        }
    }
}
