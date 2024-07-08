// ShopRepository.cs
using IdentityDemo.Data;
using IdentityDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly AppDbContext _context;

        public ShopRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> ShopCount()
        {
            return _context.Shops.Count();
        }
        public IEnumerable<ShopModel> GetAllShopsAsync()
        {
            return _context.Shops.ToList();
        }

        public async Task<ShopModel> GetShopByIdAsync(int shopId)
        {
            return await _context.Shops.FirstOrDefaultAsync(s => s.ShopId == shopId);
        }

        public async Task AddShopAsync(ShopModel shop)
        {
            await _context.Shops.AddAsync(shop);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateShopAsync(ShopModel shop)
        {
            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteShopAsync(int shopId)
        {
            var shop = await _context.Shops.FindAsync(shopId);
            if (shop != null)
            {
                _context.Shops.Remove(shop);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ShopModel> GetShopByUserIdAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return await _context.Shops.FirstOrDefaultAsync(s => s.ShopId == user.ShopId);
        }

        public async Task<List<OrderModel>> GetOrdersByShopIdAsync(int shopId)
        {
            return await _context.Orders.Where(o => o.Shop_Id == shopId).ToListAsync();
        }

        public async Task<List<ItemModel>> GetItemsByShopIdAsync(int shopId)
        {
            return await _context.Items.Where(i => i.Shop_Id == shopId).ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetUsersByIdsAsync(List<string> ids)
        {
            return await _context.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
        }
        public async Task<int> GetNewShopIdAsync()
        {
            return  _context.Shops.Any() ? await _context.Shops.MaxAsync(s => s.ShopId) + 1 : 1;
        }
    }
}
