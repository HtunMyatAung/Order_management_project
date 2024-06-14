using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);            
            var orders = _context.Orders.Where(s=> s.Shop_Id == user.ShopId).ToList();
            return View(orders);
        }
        public async Task<IActionResult> Invoice(Dictionary<int, int> selectedItems)
        {
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
                if (tempshopid == 0)
                {
                    tempshopid = itemId;
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

            return View(invoiceViewModel); // Pass the single InvoiceViewModel to the view
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
