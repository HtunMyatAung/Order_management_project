using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using IdentityDemo.Data;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;
        public AccountRepository(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager, AppDbContext context, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _environment = environment;
            _emailService = emailService;
        }
        public async Task DeleteUser(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public IEnumerable<ApplicationUser> GetUsers()
        {
            return _context.Users.Where(u => u.Role != "Admin" && u.Deleted != 1).ToList();
        }

        public async Task<ApplicationUser> CreateNewUser(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded ? user : null;
        }

        public async Task UpdateNewUserAsync(ApplicationUser user)//normal user update
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle it appropriately
                throw new Exception("Error updating user", ex);
            }
        }
        

        public ApplicationUser GetUserById(string userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)// return boolean user update
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task<ApplicationUser> FindByEmailAsync1(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
        public async Task<bool> DeleteUserImageAsync(string userId, string imageName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            if (!string.IsNullOrEmpty(imageName) && imageName != "male_default.png")
            {
                string imagePath = Path.Combine(_environment.WebRootPath, "img/users", imageName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    user.UserImageName = null; // Clear the user image name in the database
                    await _userManager.UpdateAsync(user);
                    return true;
                }
            }

            return false;
        }

        public async Task<string> SaveUserImageAsync(string userId, IFormFile imageFile)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || imageFile == null || imageFile.Length == 0)
                return null;

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/users");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Delete old image if it exists
            if (!string.IsNullOrEmpty(user.UserImageName) && user.UserImageName != "male_default.png")
            {
                string oldImagePath = Path.Combine(uploadsFolder, user.UserImageName);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            user.UserImageName = uniqueFileName; // Update user image name in the database
            await _userManager.UpdateAsync(user);

            return uniqueFileName;
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }
    }
}
