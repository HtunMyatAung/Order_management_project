using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace IdentityDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger,AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        private byte[] GetDefaultShopImage()
        {
            // Example: Load default image file and convert to byte[]
            var imagePath = "wwwroot/img/shop/shop_default.png"; // Adjust path as per your project structure
            byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
            return imageData;

        }
        public ActionResult Show_error_loading()
        {
            return View();
        }
        public IActionResult Test64()
        {
            //Console.WriteLine("gg"+GetDefaultShopImage());
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Shop()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            var items = _context.Items.ToList();
            return View(items);
            
        }
        public IActionResult ViewCart(Dictionary<int, int> selectedItems)
        {
            // Initialize a list to store item details
            List<ItemModel> cartItems = new List<ItemModel>();

            // Iterate through the selected item IDs
            foreach (var itemId in selectedItems.Keys)
            {
                // Retrieve the item details from the database based on the item ID
                var item = _context.Items.FirstOrDefault(i => i.ItemId == itemId);

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
            
            // Pass the list of item details to the view
            return View(cartItems);
        }


        private List<ItemModel> GetCartItems()
        {
            var selectedItems = _context.Items.ToList();
            return selectedItems;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
