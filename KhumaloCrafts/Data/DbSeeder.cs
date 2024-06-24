using KhumaloCrafts.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace KhumaloCrafts.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Adding roles to the database
            if (!await roleManager.RoleExistsAsync(Roles.Admin.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            }
            if (!await roleManager.RoleExistsAsync(Roles.User.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            }

            // Create admin user
            var adminEmail = "admin@gmail.com";
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var userInDb = await userManager.FindByEmailAsync(adminEmail);
            if (userInDb == null)
            {
                await userManager.CreateAsync(adminUser, "@Admin123");
                await userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
            }
        }
    }
}
