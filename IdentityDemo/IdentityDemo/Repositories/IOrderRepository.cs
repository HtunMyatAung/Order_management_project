using IdentityDemo.Models;

namespace IdentityDemo.Repositories
{
    public interface IOrderRepository
    {
        Task<List<OrderModel>> GetAllOrdersAsync();
        Task<int> GetNewOrderDetailIdAsync();
        Task<int> GetNewOrderIdAsync();
        Task<List<ItemModel>> GetItemsByIdsAsync(List<int> itemIds);
        Task<ShopModel> GetShopByIdAsync(int shopId);
        Task<ApplicationUser> GetCurrentUserAsync(HttpContext httpContext);
        Task SaveOrderAsync(OrderModel order, List<OrderDetailModel> orderDetails);
        Task<OrderModel> GetOrderByIdAsync(int orderId);
        Task DeleteOrderAsync(OrderModel order);





    }
}
