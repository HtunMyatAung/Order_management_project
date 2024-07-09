using IdentityDemo.Data;
using IdentityDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> AllItemCount()
        {
            return _context.Items.Count();
        }
        public async Task<List<ItemModel>> GetAllItemsAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<List<ShopModel>> GetAllShopsAsync()
        {
            return await _context.Shops.ToListAsync();
        }

        public async Task<ItemModel> GetItemByIdAsync(int itemId)
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.ItemId == itemId);
        }

        public async Task<ShopModel> GetShopByIdAsync(int shopId)
        {
            return await _context.Shops.SingleOrDefaultAsync(s => s.ShopId == shopId);
        }

        public async Task AddItemAsync(ItemModel item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(ItemModel item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int itemId)
        {
            var item = await GetItemByIdAsync(itemId);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetMaxItemIdAsync()
        {
            return await _context.Items.MaxAsync(s => s.ItemId);
        }
    }
}
