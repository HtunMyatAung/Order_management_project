using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityDemo.Services
{
    public interface IAdminService
    {
        Task<TableCountsViewModel> GetAdminDashboardDataAsync();
        Task<List<ApplicationUser>> GetForgotPasswordUsersAsync();
        Task<List<ShopViewModel>> GetShopViewModelsAsync();
        Task<UpdateUserViewModel> GetUpdateUserViewModelAsync(string userId);
        Task UpdateUserAsync(UpdateUserViewModel model);
        Task DeleteUserAsync(string userId);
        Task<IdentityResult> ResetPasswordAsync(string userId);
        Task<IdentityResult> ChangePasswordAsync(string userId,UpdateUserViewModel model);
    }
}
