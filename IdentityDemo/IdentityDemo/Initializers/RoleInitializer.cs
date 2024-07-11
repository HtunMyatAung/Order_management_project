using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityDemo.Initializers
{
    public class RoleInitializer:IRoleInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleInitializer(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedRolesAsync()
        {
            // Define roles
            var roles = new[] { "Admin", "Owner", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create a default admin user
            var adminUserEmail = "admin@gmail.com";
                var adminUser = await _userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                adminUser = new ApplicationUser
                {
                    UserName = adminUserEmail,
                    Email = adminUserEmail,
                    ShopId = 0,
                    Role="Admin",
                    Deleted=0,
                    UserImageName= "male_default.png"

                };
                    await _userManager.CreateAsync(adminUser, "Admin123!@#");
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
        }
    }
}
