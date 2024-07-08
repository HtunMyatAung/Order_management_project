using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<List<OrderModel>> GetAllOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }
    public async Task<int> GetNewOrderDetailIdAsync()
    {
        return _context.OrderDetails.Any() ? await _context.OrderDetails.MaxAsync(s => s.OrderDetailId) :0;
    }
    public async Task<int> GetNewOrderIdAsync()
    {
        return _context.Orders.Any() ? await _context.Orders.MaxAsync(s => s.OrderID) + 1 : 1;
    }

    public async Task<List<ItemModel>> GetItemsByIdsAsync(List<int> itemIds)
    {
        return await _context.Items.Where(i => itemIds.Contains(i.ItemId)).ToListAsync();
    }

    public async Task<ShopModel> GetShopByIdAsync(int shopId)
    {
        return await _context.Shops.FindAsync(shopId);
    }

    public async Task<ApplicationUser> GetCurrentUserAsync(HttpContext httpContext)
    {
        return await _userManager.GetUserAsync(httpContext.User);
    }


    public async Task SaveOrderAsync(OrderModel order, List<OrderDetailModel> orderDetails)
    {
        _context.Orders.Add(order);
        _context.OrderDetails.AddRange(orderDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<OrderModel> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.OrderID == orderId);
    }

    public async Task DeleteOrderAsync(OrderModel order)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}
