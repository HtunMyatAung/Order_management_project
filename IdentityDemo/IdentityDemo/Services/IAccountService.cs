using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using System.Security.Claims;

namespace IdentityDemo.Services
{
    public interface IAccountService
    {

        Task<UpdateUserViewModel> GetUpdateUserViewModelAsync(string userId);//from admin call
        Task DeleteUserAsync(string userId);//from admin call
        IEnumerable<ApplicationUser> GetAllUser();//from admin call
        void SendOTP(string email);//
        void ChangePasswordSendEmail(string email);//from user call
        Task SendRegisterConfirmEmail(string email, string otpcode);//from user call
        Task<bool> CreateNewUser(RegisterViewModel model);//from user call
        void UpdateUserAsync1(ApplicationUser user);//from send email call
        void LoginRole(LoginViewModel model);// user,owner,admin call
        Task<ApplicationUser> GetUserByIdAsync(string userId);//
        Task<ProfileViewModel> GetUserProfileAsync(ClaimsPrincipal user);//user,owner call
        Task<UpdateUserViewModel> GetUpdateUserAsync(ClaimsPrincipal user);//
        Task<bool> UpdateUserAsync(ApplicationUser user, UpdateUserViewModel model);
        Task<ApplicationUser> FindUserByEmailAsync(string email);//send email,register,login calls
        Task<bool> ResetPasswordAsync(ApplicationUser user, string newPassword);//admin,user,owner calls
        Task<bool> UpdateUserProfileAsync(ApplicationUser user, ProfileViewModel model);//user,owner calls
        Task<bool> DeleteUserImageAsync(string userId, string imageName);//update user profile call
        Task<string> SaveUserImageAsync(string userId, IFormFile imageFile);//update user profile call
        

    }
}
