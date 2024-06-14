using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
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
        public IActionResult Owner_Index()
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

        [HttpPost]
        public async Task<IActionResult> UpdateShop(ShopModel shop, IFormFile profileImage)
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
                if (profileImage != null && profileImage.Length > 0)
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

                // Update the shop entity in the database
                _context.Shops.Update(shopToUpdate);
                await _context.SaveChangesAsync();

                // Redirect to Owner_Index action in Shop controller after successful update
                return RedirectToAction("Owner_Index", "Shop");
            }

            // If ModelState is not valid, return to the view with validation errors
            return View(shop);
        }
        public IActionResult DeleteShop(int shopid)
        {
            var std = _context.Shops.FirstOrDefault(s => s.ShopId == shopid);
            if (std == null)
            {
                return NotFound();
            }
            
            return View(std);
        }

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
    }
}
