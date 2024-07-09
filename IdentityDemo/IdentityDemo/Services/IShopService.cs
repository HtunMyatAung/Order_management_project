// IShopService.cs
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityDemo.Services
{
    public interface IShopService
    {
        Task<int> ShopCount();
        Task<List<ShopViewModel>> GetShopsNOwnersAsync();
        IEnumerable<ShopModel> GetAllShopsAsync();
        Task<ShopModel> GetShopByIdAsync(int shopId);
        Task AddShopAsync(ShopViewModel shopViewModel);
        Task UpdateShopAsync(ShopViewModel shopViewModel);
        Task DeleteShopAsync(int shopId);
        Task<DashboardViewModel> GetOwnerDashboardAsync(string userId);
    }
}
