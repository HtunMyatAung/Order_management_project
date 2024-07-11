using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOrderService
{
    Task<List<OrderModel>> GetAllOrders();
    Task<List<OrderViewModel>> GetOrderNUserByShopIdAsync(int shopId);//owner call
    Task<List<OrderModel>> GetAllOrdersByShopIdAsync(int shopId);//owner call
    Task<InvoiceViewModel> PrepareInvoiceAsync(Dictionary<int, int> selectedItems, HttpContext httpContext);//user call
    Task SaveOrderAsync(InvoiceViewModel invoiceViewModel);//user call
    Task DeleteOrderAsync(int orderId);//
    Task SendInvoiceEmailAsync(string htmlContent, ApplicationUser user);//user call
}
