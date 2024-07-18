using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Repositories;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ICategoryRepository _categroyRepository;
        private readonly AppDbContext _context;
        public ItemService(IItemRepository itemRepository, UserManager<ApplicationUser> userManager,ICategoryRepository categoryRepository,AppDbContext context,IWebHostEnvironment environment)
        {
            _itemRepository = itemRepository;
            _userManager = userManager;
            _categroyRepository = categoryRepository;
            _context = context;
            _environment = environment;
        }
        public async Task<SingleItemViewModel> getSingleItemViewModelAsync(int shopId)
        {
            var categories = await _categroyRepository.GetCategoryNamesAsync();
            return new SingleItemViewModel()
            {
                Shop_Id = shopId,
                Categories = categories
            };
            
        }
        public async Task<int> AllItemCount()
        {
            return await _itemRepository.AllItemCount();
        }
        public async Task<ItemsViewModel> GetHomePageItemsAsync()
        {
            var shops = await _itemRepository.GetAllShopsAsync();
            var items = await _itemRepository.GetAllItemsAsync();
            var category = await _categroyRepository.GetCategoryNamesAsync();
            return new ItemsViewModel
            {
                Shops = shops,
                Items = items,
                Categories = category
            };
        }
        public async Task<List<ItemModel>> GetAllItemsByShopIdAsync(int shopId)
        {
            var items = await _itemRepository.GetAllItemsAsync();
            return items.Where(s => s.Shop_Id == shopId).ToList();
        }
        public async Task<ItemsViewModel> GetItemNShopByShopIdAsync(int shopId)
        {
            var shop = await _itemRepository.GetShopByIdAsync(shopId);
            var items = await _itemRepository.GetAllItemsAsync();
            
            return new ItemsViewModel
            {
                Shop = shop,
                Items = items.Where(i => i.Shop_Id == shopId).ToList()
            };
        }

        public async Task<SingleItemViewModel> GetItemForUpdateAsync(int itemId, string userId)
        {
            var item = await _itemRepository.GetItemByIdAsync(itemId);
            var user = await _userManager.FindByIdAsync(userId);
            if (item == null || item.Shop_Id != user.ShopId)
            {
                return null;
            }
            return new SingleItemViewModel
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                ItemPrice = item.ItemPrice,
                ItemQuantity = item.ItemQuantity,
                ItemChangedPrice = item.ItemChangedPrice,
                Shop_Id = item.Shop_Id,
                Discount_rate = item.Discount_rate,
                Discount_price = item.Discount_price,
                Category = item.Category,
            };
        }

        public async Task UpdateItemAsync(SingleItemViewModel updateItem, string uniqueFileName)
        {
            // Fetch the item from the repository by its ID
            var item = await _itemRepository.GetItemByIdAsync(updateItem.ItemId);
            if (item == null)
            {
                throw new ArgumentException("Item not found", nameof(updateItem.ItemId));
            }

            // Handle the image update if a new file name is provided
            if (!string.IsNullOrEmpty(uniqueFileName))
            {
                // Check if there is an existing image that is not the default image
                if (!string.IsNullOrEmpty(item.ItemImageName) && item.ItemImageName != "item_default.png")
                {
                    // Construct the path to the old image
                    string oldImagePath = Path.Combine(_environment.WebRootPath, "img/items", item.ItemImageName);
                    if (File.Exists(oldImagePath))
                    {
                        try
                        {
                            File.Delete(oldImagePath);
                        }
                        catch (Exception ex)
                        {
                            // Log the error or handle it as needed
                            //_logger.LogError(ex, "Error deleting old image file.");
                            throw;
                        }
                    }
                }

                // Update the item's image name to the new unique file name
                item.ItemImageName = uniqueFileName;
            }

            // Update other properties of the item
            item.ItemName = updateItem.ItemName;
            item.ItemPrice = updateItem.ItemPrice;
            item.ItemQuantity = updateItem.ItemQuantity;
            item.ItemChangedPrice = updateItem.ItemChangedPrice;
            item.Discount_rate = updateItem.Discount_rate;
            item.Discount_price = updateItem.Discount_price;
            item.ItemUpdatedDate = DateTime.Now;

            // Save the updated item to the repository
            await _itemRepository.UpdateItemAsync(item);
        }

        public async Task AddItemAsync(SingleItemViewModel item, string uniqueFileName)
        {
            var newItemId = await _itemRepository.GetMaxItemIdAsync() + 1;
            var newItem = new ItemModel
            {
                ItemId = newItemId,
                ItemName = item.ItemName,
                ItemPrice = item.ItemPrice,
                ItemQuantity = item.ItemQuantity,
                ItemCreatedDate = DateTime.Now,
                ItemUpdatedDate = DateTime.Now,
                ItemChangedPrice = item.ItemPrice,
                Shop_Id = item.Shop_Id,
                Discount_rate = 0,
                Discount_price = 0,
                ItemImageName = uniqueFileName,
                Category= item.Category,
            };
            var updatecategory = await _categroyRepository.GetCategoryByNameAsync(item.Category);
            updatecategory.Item_count = updatecategory.Item_count + 1;
            await _categroyRepository.UpdateCategoryAsync(updatecategory);
            await _itemRepository.AddItemAsync(newItem);
        }

        public async Task DeleteItemAsync(int itemId)
        {
            await _itemRepository.DeleteItemAsync(itemId);
        }
    }
}
