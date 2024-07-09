using IdentityDemo.Models;
using IdentityDemo.ViewModels;

namespace IdentityDemo.Services
{
    public interface IItemService
    {
        Task<int> AllItemCount();
        Task<List<ItemModel>> GetAllItemsByShopIdAsync(int shopId);
        Task<ItemsViewModel> GetHomePageItemsAsync();
        Task<ItemsViewModel> GetItemNShopByShopIdAsync(int shopId);
        Task<SingleItemViewModel> GetItemForUpdateAsync(int itemId, string userId);
        Task UpdateItemAsync(SingleItemViewModel updateItem, string uniqueFileName);
        Task AddItemAsync(SingleItemViewModel item, string uniqueFileName);
        Task DeleteItemAsync(int itemId);
    }
}
