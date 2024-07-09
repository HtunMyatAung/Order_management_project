// IShopRepository.cs
using IdentityDemo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityDemo.Repositories
{
    public interface IShopRepository
    {
        Task<int> ShopCount();
        Task<int> GetNewShopIdAsync();
        IEnumerable<ShopModel> GetAllShopsAsync();
        Task<ShopModel> GetShopByIdAsync(int shopId);
        Task AddShopAsync(ShopModel shop);
        Task UpdateShopAsync(ShopModel shop);
        Task DeleteShopAsync(int shopId);
        Task<ShopModel> GetShopByUserIdAsync(string userId);
        Task<List<OrderModel>> GetOrdersByShopIdAsync(int shopId);
        Task<List<ItemModel>> GetItemsByShopIdAsync(int shopId);
        Task<List<ApplicationUser>> GetUsersByIdsAsync(List<string> ids);
    }
}
