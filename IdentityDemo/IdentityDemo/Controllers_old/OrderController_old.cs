using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;

namespace IdentityDemo.Controllers
{
    public class OrderController_old : Controller
    {
        public readonly AppDbContext _context;
        public readonly UserManager<ApplicationUser> _userManager;       
        private readonly IEmailService _emailService;
        
        public OrderController_old(AppDbContext context, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;            
        }                
        [HttpPost]
        public async Task<IActionResult> SendInvoiceEmail(string htmlContent)
        {
            try
            {
                Console.WriteLine(htmlContent);
                var currentUser = await _userManager.GetUserAsync(User);
                var user_mail = currentUser.Email;
                string toEmail = user_mail;
                string subject = "uab invoice";

                await _emailService.SendEmailAsync(toEmail, subject, htmlContent);
                TempData["test"] = "hahaha";//test
                ViewBag.Message = "Email sent successfully!";//test
                return RedirectToAction("SaveInvoice", "Order");
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> Order_confirm(Dictionary<int,int> selectedItems)
        {
            TempData["SelectedItems"] = JsonConvert.SerializeObject(selectedItems);
            if (selectedItems == null || selectedItems.Count == 0)
            {
                return RedirectToAction("HomePageItems", "Item");
            }

            int newOrderId = _context.Orders.Any() ? _context.Orders.Max(s => s.OrderID) + 1 : 1;

            List<ItemModel> cartItems = new List<ItemModel>();
            int tempShopId = 0;

            // Fetch all item IDs to minimize database round-trips
            var itemIds = selectedItems.Keys.ToList();
            var items = await _context.Items.Where(i => itemIds.Contains(i.ItemId)).ToListAsync();

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

            var shopData = await _context.Shops.FindAsync(tempShopId);
            if (shopData == null)
            {
                return NotFound(); // Handle case where shop data is not found
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var invoiceViewModel = new InvoiceViewModel
            {
                User = currentUser,
                OrderId = newOrderId,
                Shop = shopData,
                OrderDate = DateTime.Now,
                Items = cartItems
            };

            // Store InvoiceViewModel in TempData
            TempData["InvoiceViewModel"] = JsonConvert.SerializeObject(invoiceViewModel);

            return View(invoiceViewModel); // Pass the InvoiceViewModel to the view
            
        }

        //[HttpGet]
        public async Task<IActionResult> Invoice()
        {
            var selectedItemsJson = TempData["SelectedItems"].ToString();
            var selectedItems = JsonConvert.DeserializeObject<Dictionary<int, int>>(selectedItemsJson);//retrieve invoice dictionary data
            try
            {
                if (selectedItems == null || selectedItems.Count == 0)
                {
                    return RedirectToAction("HomePageItems", "Item");
                }
                int newOrderId = _context.Orders.Any() ? _context.Orders.Max(s => s.OrderID) + 1 : 1;
                List<ItemModel> cartItems = new List<ItemModel>();
                int tempShopId = 0;
                // Fetch all item IDs to minimize database round-trips
                var itemIds = selectedItems.Keys.ToList();
                var items = await _context.Items.Where(i => itemIds.Contains(i.ItemId)).ToListAsync();
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
                var shopData = await _context.Shops.FindAsync(tempShopId);
                if (shopData == null)
                {
                    return NotFound(); // Handle case where shop data is not found
                }
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                var invoiceViewModel = new InvoiceViewModel
                {
                    User = currentUser,
                    OrderId = newOrderId,
                    Shop = shopData,
                    OrderDate = DateTime.Now,
                    Items = cartItems
                };
                // Store InvoiceViewModel in TempData
                TempData["InvoiceViewModel"] = JsonConvert.SerializeObject(invoiceViewModel);
                return View(invoiceViewModel); // Pass the InvoiceViewModel to the view
            }
            catch (Exception ex)
            {
                // Log the exception
                //_logger.LogError(ex, "Error generating invoice");

                // Optionally, return a specific error view or redirect
                return RedirectToAction("Error", "Home");
            }
        }

        //[HttpPost]
        public IActionResult SaveInvoice()
        {
            if (TempData["InvoiceViewModel"] == null)
            {
                // Handle case where InvoiceViewModel is null
                return BadRequest("Invalid data received");
            }

            // Example: Save or process the invoiceViewModel data as needed
            var invoiceViewModelJson = TempData["InvoiceViewModel"].ToString();
            InvoiceViewModel invoiceViewModel = JsonConvert.DeserializeObject<InvoiceViewModel>(invoiceViewModelJson);
            // Calculate the sum of the total item prices
            decimal totalSum = invoiceViewModel.Items.Sum(item => item.ItemChangedPrice * item.ItemQuantity);
            var itemidlist= invoiceViewModel.Items.ToList();
            OrderModel orderModel = new OrderModel
            {
               OrderID = invoiceViewModel.OrderId,
               User_Id=invoiceViewModel.User.Id,
               OrderPrice=totalSum,
               OrderDate=DateTime.Now,
               Shop_Id=invoiceViewModel.Shop.ShopId
            };
            int maxorderdetailId = _context.OrderDetails.Any() ? _context.OrderDetails.Max(s => s.OrderDetailId) : 0;
            int neworderdetailId = maxorderdetailId + 1;
            // Create a list of OrderDetail entities
            List<OrderDetailModel> orderDetails = invoiceViewModel.Items.Select(item => new OrderDetailModel
            {
                OrderDetailId = ++maxorderdetailId,
                OrderId = invoiceViewModel.OrderId,
                ItemId = item.ItemId,
                Item_quantity=item.ItemQuantity,
                Initial_price=item.ItemPrice,
                Final_price=item.ItemChangedPrice

            }).ToList();
            // Add the order details to the database context
            _context.OrderDetails.AddRange(orderDetails);
            //Add order to database
            _context.Orders.Add(orderModel);
            _context.SaveChanges();
            //invoiceViewModel.User.Id
            //return Ok("Invoice data saved successfully");
            return RedirectToAction("HomePageItems", "Item");
        }
        [HttpPost]
        public IActionResult DeleteOrder(int orderid)
        {
            var std = _context.Orders.FirstOrDefault(s => s.OrderID == orderid);
            if(std == null)
            {
                return NotFound();
            }
            _context.Orders.Remove(std);
            _context.SaveChanges();
            return RedirectToAction("Owner_order_list","Shop");
        }
        
    }
}
