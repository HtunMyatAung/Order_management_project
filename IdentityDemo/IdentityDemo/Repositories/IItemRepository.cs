using IdentityDemo.Models;

namespace IdentityDemo.Repositories
{
    public interface IItemRepository
    {
        Task<int> AllItemCount();
        Task<List<ItemModel>> GetAllItemsAsync();
        Task<List<ShopModel>> GetAllShopsAsync();
        Task<ItemModel> GetItemByIdAsync(int itemId);
        Task<ShopModel> GetShopByIdAsync(int shopId);
        Task AddItemAsync(ItemModel item);
        Task UpdateItemAsync(ItemModel item);
        Task DeleteItemAsync(int itemId);
        Task<int> GetMaxItemIdAsync();
    }
}
