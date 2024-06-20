using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Controllers
{
    public class ShopController : Controller
    {
        //private readonly IShopServices _shopService;

        /*public ShopController(IShopServices shopService)
        {
            _shopService = shopService;
        }*/
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ShopController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        /* public async Task<ShopModel> GetShopDataAsync(string shopId)
         {
             // Fetch shop data based on userId
             //return await _context.Shops.FirstOrDefaultAsync(s => s.ShopId== shopId);
         }*/
        [Authorize(Roles ="Owner")]
        public IActionResult Owner_Item_List()
        {
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            var std = _context.Items.Where(s => s.Shop_Id == user.ShopId).ToList();
            if (std == null)
            {
                return NotFound();
            }
            return View(std);
        }
        public IActionResult Owner_order_list()
        {
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                // Handle error: user not found
                return NotFound();
            }

            // Fetch orders for the logged-in owner
            var orders = _context.Orders
                                 .Where(o => o.Shop_Id == user.ShopId)
                                 .ToList();

            // Create a list of OrderViewModel with User_Name
            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                OrderID = o.OrderID,
                OrderDate = o.OrderDate,
                OrderPrice = o.OrderPrice,
                Shop_Id = o.Shop_Id,
                User_Name = _context.Users.FirstOrDefault(u => u.Id == o.User_Id)?.UserName
            }).ToList();

            return View(orderViewModels);
        }


        [Authorize(Roles = "Owner")]
        [HttpGet]
        public IActionResult UpdateShop()
        {
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            var shop = _context.Shops.FirstOrDefault(s => s.ShopId ==user.ShopId );
            if (shop == null)
            {
                return NotFound();
            }
            return View(shop);
        }
        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> UpdateShop(ShopModel shop)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

                if (user == null)
                {
                    return NotFound(); // Handle case where user is not found
                }

                var shopToUpdate = await _context.Shops.FirstOrDefaultAsync(s => s.ShopId == user.ShopId);

                if (shopToUpdate == null)
                {
                    return NotFound(); // Handle case where shop is not found
                }

                shopToUpdate.ShopName = shop.ShopName;
                shopToUpdate.ShopPhone = shop.ShopPhone;
                shopToUpdate.ShopDescription = shop.ShopDescription;
                shopToUpdate.ShopAddress = shop.ShopAddress;
                shopToUpdate.ShopEmail = shop.ShopEmail;

                // Handle ShopImage update
                /*if (profileImage != null && profileImage.Length > 0)
                {
                    // Generate a unique file name for the image
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);

                    // Define the folder where the image will be stored (wwwroot/img/shop)
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "shop");

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Combine the uploads folder with the unique file name
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Copy the uploaded file to the file path
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(fileStream);
                    }

                    // Update the ShopImage property in the shopToUpdate object
                    shopToUpdate.ShopImage = uniqueFileName; // Store only the file name or relative path
                }
                */
                // Update the shop entity in the database
                _context.Shops.Update(shopToUpdate);
                await _context.SaveChangesAsync();

                // Redirect to Owner_Index action in Shop controller after successful update
                return RedirectToAction("Owner_dashboard", "Shop");
            }

            // If ModelState is not valid, return to the view with validation errors
            return View(shop);
        }
        [Authorize(Roles = "Owner")]
        public IActionResult DeleteShop(int shopid)
        {
            var std = _context.Shops.FirstOrDefault(s => s.ShopId == shopid);
            if (std == null)
            {
                return NotFound();
            }
            
            return View(std);
        }
        [Authorize(Roles = "Owner")]
        [HttpPost]
        public IActionResult DeleteShop(ShopModel shop)
        {
            if (ModelState.IsValid)
            {
                var std = _context.Shops.FirstOrDefault(s => s.ShopId == shop.ShopId);
                if (std == null)
                {
                    return NotFound();
                }
                _context.Shops.Remove(std);
                _context.SaveChanges();
                return RedirectToAction("Login","Account");
            }
            return View();
        }
        [Authorize(Roles = "Owner")]
        public IActionResult Owner_dashboard()
        {
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null || user.ShopId == null)
            {
                return NotFound(); // Handle case where user or shop is not found
            }

            var shopId = user.ShopId;

            var orders = _context.Orders
                                 .Where(o => o.Shop_Id == user.ShopId)
                                 .ToList();

            var items = _context.Items
                                .Where(i => i.Shop_Id == shopId)
                                .ToList();
            var ordercount=orders.Count();
            var itemcount=items.Count();
            // Get distinct customer IDs from orders
            var customerIds = orders.Select(o => o.User_Id).Distinct().ToList();
            var customercount= customerIds.Count();
            // Fetch users whose IDs are in the customerIds list
            var customers = _context.Users
                                    .Where(c => customerIds.Contains(c.Id))
                                    .ToList();
            // Prepare order data for the chart
            var orderData = orders.Select(o => new OrderModel
            {
                OrderDate = o.OrderDate,
                OrderPrice = o.OrderPrice
            })
            .OrderBy(o => o.OrderDate)
            .ToList();
            var orderCountsByDay = orders.GroupBy(o => o.OrderDate.Date)
                             .Select(g => new { Date = g.Key, Count = g.Count() })
                             .OrderBy(d => d.Date)
                             .ToList();
            //Console.WriteLine();
            //Console.WriteLine("haha" + orderCountsByDay==null);
            // Extract labels (dates) and data (order counts) for the chart
            var labels = orderCountsByDay.Select(d => d.Date.ToString("yyyy-MM-dd")).ToArray();
            var data = orderCountsByDay.Select(d => d.Count).ToArray();
            
            // _context.Users.Where(u => u.Role != null && u.Role != "Admin").ToList();
            var ownerdashboardViewModel = new DashboardViewModel
            {
                OrderCount = ordercount,
                ItemCount = itemcount,
                CustomerCount= customercount,
                OrderData = orders,
                ItemData = items,
                UserData = customers,
                Labels= labels,
                Datas= data
            };

            return View(ownerdashboardViewModel);
        }

    }
}
