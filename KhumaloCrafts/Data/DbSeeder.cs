using KhumaloCrafts.Models;
using Microsoft.AspNetCore.Identity;

namespace KhumaloCrafts.Data
{
    public class DbSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));

            var adminUser = new ApplicationUser
            {
                UserName = "admin@khumalo.com",
                Email = "admin@khumalo.com",
                EmailConfirmed = true
            };

            var userInDb = await userManager.FindByEmailAsync(adminUser.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}