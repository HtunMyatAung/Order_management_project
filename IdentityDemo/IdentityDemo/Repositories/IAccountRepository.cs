using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using System.Security.Claims;

namespace IdentityDemo.Repositories
{
    public interface IAccountRepository
    {
        Task<ApplicationUser> CreateNewUser(ApplicationUser user, string password);
        Task UpdateNewUser(ApplicationUser user);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<ApplicationUser> FindByEmailAsync1(string email);
        Task<bool> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<bool> DeleteUserImageAsync(string userId, string imageName);
        Task<string> SaveUserImageAsync(string userId, IFormFile imageFile);
    }
}
