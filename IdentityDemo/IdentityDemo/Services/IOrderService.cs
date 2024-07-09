using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOrderService
{
    Task<List<OrderModel>> GetAllOrders();
    Task<List<OrderViewModel>> GetOrderNUserByShopIdAsync(int shopId);
    Task<List<OrderModel>> GetAllOrdersByShopIdAsync(int shopId);
    Task<InvoiceViewModel> PrepareInvoiceAsync(Dictionary<int, int> selectedItems, HttpContext httpContext);
    Task SaveOrderAsync(InvoiceViewModel invoiceViewModel);
    Task DeleteOrderAsync(int orderId);
    Task SendInvoiceEmailAsync(string htmlContent, ApplicationUser user);
}
