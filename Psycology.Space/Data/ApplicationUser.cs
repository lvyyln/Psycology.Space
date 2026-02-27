using Microsoft.AspNetCore.Identity;

namespace Psycology.Space.Data;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
