using Microsoft.AspNetCore.Identity;

namespace DDJ.Infrastructure.Identity.Entities;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }

    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
}
