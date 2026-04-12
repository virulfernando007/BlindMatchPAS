using BlindMatchPAS.Models;
using Microsoft.AspNetCore.Identity;

namespace BlindMatchPAS.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Student", "Supervisor", "ModuleLeader", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName        = "admin@blindmatch.ac.lk",
                Email           = "admin@blindmatch.ac.lk",
                FullName        = "System Administrator",
                Department      = "IT Services",
                EmailConfirmed  = true
            }, "Admin@123!", "Admin");

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName        = "moduleleader@blindmatch.ac.lk",
                Email           = "moduleleader@blindmatch.ac.lk",
                FullName        = "Dr. Module Leader",
                Department      = "Computing",
                EmailConfirmed  = true
            }, "Leader@123!", "ModuleLeader");

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName        = "supervisor@blindmatch.ac.lk",
                Email           = "supervisor@blindmatch.ac.lk",
                FullName        = "Dr. Sample Supervisor",
                Department      = "Computer Science",
                EmailConfirmed  = true
            }, "Super@123!", "Supervisor");

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName        = "student@blindmatch.ac.lk",
                Email           = "student@blindmatch.ac.lk",
                FullName        = "Sample Student",
                StudentId       = "CS202301",
                Department      = "Computing",
                EmailConfirmed  = true
            }, "Student@123!", "Student");
        }

        private static async Task SeedUserAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string password,
            string role)
        {
            if (await userManager.FindByEmailAsync(user.Email!) == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}