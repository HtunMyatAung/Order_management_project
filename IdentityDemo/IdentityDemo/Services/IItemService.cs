using IdentityDemo.Models;
using IdentityDemo.ViewModels;

namespace IdentityDemo.Services
{
    public interface IItemService
    {
        Task<int> AllItemCount();//from owner,admin calls
        Task<List<ItemModel>> GetAllItemsByShopIdAsync(int shopId);//from user view ,owner view show calls
        Task<ItemsViewModel> GetHomePageItemsAsync();// home page call
        Task<ItemsViewModel> GetItemNShopByShopIdAsync(int shopId);//owner item list call
        Task<SingleItemViewModel> GetItemForUpdateAsync(int itemId, string userId);//owner call
        Task UpdateItemAsync(SingleItemViewModel updateItem, string uniqueFileName);//owner call
        Task AddItemAsync(SingleItemViewModel item, string uniqueFileName);//owner call
        Task DeleteItemAsync(int itemId);//owner call
    }
}
