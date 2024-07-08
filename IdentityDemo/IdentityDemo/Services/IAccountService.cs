using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using System.Security.Claims;

namespace IdentityDemo.Services
{
    public interface IAccountService
    {
        void SendOTP(string email);
        void ChangePasswordSendEmail(string email);
        void SendRegisterConfirmEmail(string email, string otpcode);
        Task<bool> CreateNewUser(RegisterViewModel model);
        void UpdateUserAsync1(ApplicationUser user);
        void LoginRole(LoginViewModel model);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ProfileViewModel> GetUserProfileAsync(ClaimsPrincipal user);
        Task<UpdateUserViewModel> GetUpdateUserAsync(ClaimsPrincipal user);
        Task<bool> UpdateUserAsync(ApplicationUser user, UpdateUserViewModel model);
        Task<ApplicationUser> FindUserByEmailAsync(string email);
        Task<bool> ResetPasswordAsync(ApplicationUser user, string newPassword);
        Task<bool> UpdateUserProfileAsync(ApplicationUser user, ProfileViewModel model);
        Task<bool> DeleteUserImageAsync(string userId, string imageName);
        Task<string> SaveUserImageAsync(string userId, IFormFile imageFile);
        

    }
}
