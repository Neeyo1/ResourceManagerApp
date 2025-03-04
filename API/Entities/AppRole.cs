using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppRole : IdentityRole<int>
{
    //AppRole - AppUserRoles
    public ICollection<AppUserRole> UserRoles { get; set; } = [];
}
