using Microsoft.AspNetCore.Identity;

namespace FHN_TTNDA.Web.Models.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public ApplicationRole() : base() { }
    public ApplicationRole(string roleName) : base(roleName) { }
}
