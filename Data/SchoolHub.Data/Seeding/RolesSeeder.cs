namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    using SchoolHub.Common;
    using SchoolHub.Data.Models;

    internal class RolesSeeder : ISeeder
    {
        private const string AdminEmail = "admin@crs.com";

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await SeedRoleAsync(userManager, roleManager, GlobalConstants.AdministratorRoleName);
            await SeedRoleAsync(userManager, roleManager, GlobalConstants.TeacherRoleName);
            await SeedRoleAsync(userManager, roleManager, GlobalConstants.StudentRoleName);
        }

        private static async Task SeedRoleAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }

                var user = new ApplicationUser
                {
                    Email = AdminEmail,
                    UserName = AdminEmail,
                };

                await userManager.CreateAsync(user, "admin21");
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
