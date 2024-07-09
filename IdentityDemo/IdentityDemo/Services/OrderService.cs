using IdentityDemo.Models;
using IdentityDemo.Repositories;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;
    public OrderService(IOrderRepository orderRepository, IEmailService emailService, UserManager<ApplicationUser> userManager)
    {
        _orderRepository = orderRepository;
        _emailService = emailService;
        _userManager = userManager;
    }
    public Task<List<OrderModel>> GetAllOrders()
    {
        return _orderRepository.GetAllOrdersAsync();
    }
    public async Task<List<OrderViewModel>> GetOrderNUserByShopIdAsync(int shopId)
    {
        var all_orders = await _orderRepository.GetAllOrdersAsync();
        var orders= all_orders.Where(s => s.Shop_Id == shopId).ToList();
        var orderViewModels = new List<OrderViewModel>();
       
        foreach (var order in orders)
        {
            var user = await _userManager.FindByIdAsync(order.User_Id);
            orderViewModels.Add(new OrderViewModel
            {
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                OrderPrice = order.OrderPrice,
                Shop_Id = order.Shop_Id,
                User_Name = user?.UserName
            });
        }

        return orderViewModels;
    }
    public  async Task<List<OrderModel>> GetAllOrdersByShopIdAsync(int shopId)
    {
        var orders = await _orderRepository.GetAllOrdersAsync();

        return orders.Where(s => s.Shop_Id == shopId).ToList();
    }
    public async Task<InvoiceViewModel> PrepareInvoiceAsync(Dictionary<int, int> selectedItems, HttpContext httpContext)
    {
        if (selectedItems == null || selectedItems.Count == 0)
        {
            return null;
        }

        int newOrderId = await _orderRepository.GetNewOrderIdAsync();
        List<ItemModel> cartItems = new List<ItemModel>();
        int tempShopId = 0;

        var itemIds = selectedItems.Keys.ToList();
        var items = await _orderRepository.GetItemsByIdsAsync(itemIds);

        foreach (var itemId in itemIds)
        {
            var tempItem = items.FirstOrDefault(i => i.ItemId == itemId);
            if (tempItem != null)
            {
                if (tempShopId == 0)
                {
                    tempShopId = tempItem.Shop_Id;
                }

                cartItems.Add(new ItemModel
                {
                    ItemId = tempItem.ItemId,
                    ItemName = tempItem.ItemName,
                    ItemPrice = tempItem.ItemPrice,
                    ItemChangedPrice = tempItem.ItemChangedPrice,
                    ItemQuantity = selectedItems[itemId]
                });
            }
        }

        var shopData = await _orderRepository.GetShopByIdAsync(tempShopId);
        var currentUser = await _orderRepository.GetCurrentUserAsync(httpContext);

        return new InvoiceViewModel
        {
            User = currentUser,
            OrderId = newOrderId,
            Shop = shopData,
            OrderDate = DateTime.Now,
            Items = cartItems
        };
    }

    public async Task SaveOrderAsync(InvoiceViewModel invoiceViewModel)
    {
        decimal totalSum = invoiceViewModel.Items.Sum(item => item.ItemChangedPrice * item.ItemQuantity);

        OrderModel orderModel = new OrderModel
        {
            OrderID = invoiceViewModel.OrderId,
            User_Id = invoiceViewModel.User.Id,
            OrderPrice = totalSum,
            OrderDate = DateTime.Now,
            Shop_Id = invoiceViewModel.Shop.ShopId
        };
        
        int maxOrderDetailId = await _orderRepository.GetNewOrderDetailIdAsync();
        List<OrderDetailModel> orderDetails = invoiceViewModel.Items.Select(item => new OrderDetailModel
        {
            OrderDetailId = ++maxOrderDetailId,
            OrderId = invoiceViewModel.OrderId,
            ItemId = item.ItemId,
            Item_quantity = item.ItemQuantity,
            Initial_price = item.ItemPrice,
            Final_price = item.ItemChangedPrice
        }).ToList();

        await _orderRepository.SaveOrderAsync(orderModel, orderDetails);
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order != null)
        {
            await _orderRepository.DeleteOrderAsync(order);
        }
    }

    public async Task SendInvoiceEmailAsync(string htmlContent, ApplicationUser user)
    {
        string toEmail = user.Email;
        string subject = "uab invoice";
        await _emailService.SendEmailAsync(toEmail, subject, htmlContent);
    }
}
