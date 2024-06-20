using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IdentityDemo.Controllers
{
    public class OrderController : Controller
    {
        public readonly AppDbContext _context;
        public readonly UserManager<ApplicationUser> _userManager;
        public OrderController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        //[HttpGet]
        public async Task<IActionResult> Invoice(Dictionary<int, int> selectedItems)
        {
            if (selectedItems == null)
            {
                return RedirectToAction("HomePageItems", "Item");
            }
            int maxorderId = _context.Shops.Any() ? _context.Orders.Max(s => s.OrderID) : 0;
            int neworderId = maxorderId + 1;
            if (selectedItems == null || selectedItems.Count == 0)
            {
                // Handle case where selectedItems is null or empty
                // For example, redirect to a different action or return a view with an error message
                return RedirectToAction("Index", "Home"); // Redirect to home page as an example
            }
            // Initialize a list to store item details
            List<ItemModel> cartItems = new List<ItemModel>();
            int tempshopid = 0;

            // Iterate through the selected item IDs
            foreach (var itemId in selectedItems.Keys)
            {
                var tempitem=_context.Items.FirstOrDefault(i=>i.ItemId == itemId);
                if (tempshopid == 0 & tempitem !=null)
                {
                    tempshopid =tempitem.Shop_Id;
                    // Fetch the shop data based on the provided shopId
                }

                // Retrieve the item details from the database based on the item ID
                var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == itemId);

                // If the item is found, add it to the list with its quantity
                if (item != null)
                {
                    cartItems.Add(new ItemModel
                    {
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        ItemPrice = item.ItemPrice,
                        ItemQuantity = selectedItems[itemId] // Quantity from the selectedItems dictionary
                                                             // Add more item details as needed
                    });
                }
            }

            // Fetch the shop data based on the temporary shop ID
            ShopModel shopData = await _context.Shops.FirstOrDefaultAsync(s => s.ShopId == tempshopid);
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (shopData == null)
            {
                return NotFound(); // Handle case where shop data is not found
            }

            // Create the InvoiceViewModel instance
            InvoiceViewModel invoiceViewModel = new InvoiceViewModel
            {
                User = currentUser,
                OrderId=neworderId,
                Shop = shopData,
                OrderDate = DateTime.Now,
                Items = cartItems // List of items added above
            };
            // Store InvoiceViewModel in TempData
            TempData["InvoiceViewModel"] = JsonConvert.SerializeObject(invoiceViewModel);
            return View(invoiceViewModel); // Pass the single InvoiceViewModel to the view
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
            decimal totalSum = invoiceViewModel.Items.Sum(item => item.ItemPrice * item.ItemQuantity);
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
            return RedirectToAction("Index","Order");
        }
        
    }
}
