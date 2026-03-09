using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Psycology.Space.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<AvailableSlot> AvailableSlots => Set<AvailableSlot>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<IntakeResponse> IntakeResponses => Set<IntakeResponse>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}
