using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Controllers
{
    public class ShopController_old : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ShopController_old(AppDbContext context, IWebHostEnvironment environment,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }
        [Authorize(Roles ="Owner")]
        public async Task<IActionResult> Owner_Item_List()
        {
            var user = await _userManager.GetUserAsync(User);
            var std = _context.Items.Where(s => s.Shop_Id == user.ShopId).ToList();
            if (std == null)
            {
                return NotFound();
            }
            return View(std);
        }
        public async Task<IActionResult> Shop_view(int shopid)
        {
            ShopModel shop = _context.Shops.SingleOrDefault(s => s.ShopId == shopid);
            var items = await _context.Items.Where(s => s.Shop_Id == shopid).ToListAsync();
            if (!items.Any())
            {
                TempData["items"] = "hello";
            }
            var itembyshopid = new ItemsViewModel
            {
                Shop = shop,
                Items = items
            };
            return View(itembyshopid);
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
            var shopview = new ShopViewModel()
            {
                ShopId=shop.ShopId,
                ShopName=shop.ShopName,
                ShopEmail=shop.ShopEmail,
                ShopAddress=shop.ShopAddress,
                ShopPhone=shop.ShopPhone,
                ShopDescription=shop.ShopDescription,
                //ShopImageName=shop.ProfileImagePath
            };
            
            if (shop == null)
            {
                return NotFound();
            }
            return View(shopview);
        }
        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> UpdateShop(ShopViewModel shop)
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
                if (shop.ProfileImage != null)
                {
                    string uniqueFileName = null;

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(shopToUpdate.ProfileImagePath) && shopToUpdate.ProfileImagePath != "shop_default.png")
                    {
                        string oldImagePath = Path.Combine(_environment.WebRootPath, "img/shop", shopToUpdate.ProfileImagePath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    // Set the uploads folder path
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/shop");

                    // Ensure the folder exists
                    Directory.CreateDirectory(uploadsFolder);

                    // Generate a unique file name to avoid collisions
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(shop.ProfileImage.FileName);

                    // Combine folder path and file name
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Copy the uploaded file to the specified path
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await shop.ProfileImage.CopyToAsync(fileStream);
                    }
                    shopToUpdate.ProfileImagePath = uniqueFileName;
                }
                    // Update other shop details
                    shopToUpdate.ShopName = shop.ShopName;
                shopToUpdate.ShopPhone = shop.ShopPhone;
                shopToUpdate.ShopDescription = shop.ShopDescription;
                shopToUpdate.ShopAddress = shop.ShopAddress;
                shopToUpdate.ShopEmail = shop.ShopEmail;
                

                _context.Shops.Update(shopToUpdate);
                await _context.SaveChangesAsync();

                // Redirect to Owner_dashboard action in Shop controller after successful update
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
