using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityDemo.Services
{
    public interface IUserService
    {
        IEnumerable<ApplicationUser> GetAllUser();
        Task<int> GetUserCount();
        Task DeleteUserAsync(string userId);
        
    }
}
