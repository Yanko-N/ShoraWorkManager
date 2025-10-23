using Application.Core;
using Microsoft.AspNetCore.Identity;

namespace Application.Data.Roles
{
    public class SeedRoles
    {
        public static void Seed(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                roleManager.CreateAsync(new IdentityRole(AppConstants.Roles.Admin)).Wait();

                roleManager.CreateAsync(new IdentityRole(AppConstants.Roles.User)).Wait();
            }
        }
    }
}
