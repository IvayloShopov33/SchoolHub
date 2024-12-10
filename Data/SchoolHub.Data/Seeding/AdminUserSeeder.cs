namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    using SchoolHub.Common;
    using SchoolHub.Data.Models;

    public class AdminUserSeeder : ISeeder
    {
        private const string AdminEmail = "admin@crs.com";
        private const string AdminPassword = "admin21";

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var adminRole = await roleManager.FindByNameAsync(GlobalConstants.AdministratorRoleName);
            if (adminRole == null)
            {
                throw new Exception($"Role '{GlobalConstants.AdministratorRoleName}' does not exist. Please seed roles first.");
            }

            var adminUser = await userManager.FindByEmailAsync(AdminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    Email = AdminEmail,
                    UserName = AdminEmail,
                };

                var result = await userManager.CreateAsync(user, AdminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
            }
        }
    }
}
