using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace IdentityDemo.Controllers
{
    public class ItemController_old : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        public ItemController_old(AppDbContext context, IWebHostEnvironment environment,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }
        public async Task<IActionResult> HomePageItems()
        {
            var shops = await _context.Shops.ToListAsync();
            var items = await _context.Items.ToListAsync();
            var shopitems = new ItemsViewModel
            {
                Shops = shops,
                Items = items
            };
            return View(shopitems);
        }
        public async Task<IActionResult> ItemByShopid(int shopid)
        {
            ShopModel shop  = _context.Shops.SingleOrDefault(s=>s.ShopId == shopid);
            var items = await _context.Items.Where(s=>s.Shop_Id== shopid).ToListAsync();
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
        public async Task<IActionResult> UpdateItem(int itemid)
        {
            var item = _context.Items.FirstOrDefault(s => s.ItemId == itemid);
            var user = await _userManager.GetUserAsync(User);
            if (item == null)
            {
                return NotFound();
            }
            if (item.Shop_Id != user.ShopId)
            {
                return Forbid();
            }            
            var updateitem = new SingleItemViewModel()
            {
                ItemId=item.ItemId,
                ItemName=item.ItemName,
                ItemPrice=item.ItemPrice,
                ItemQuantity=item.ItemQuantity,
                ItemChangedPrice=item.ItemChangedPrice,
                Shop_Id=item.Shop_Id,
                Discount_rate=item.Discount_rate,
                Discount_price=item.Discount_price
            };
            return View(updateitem);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateItem(SingleItemViewModel updateItem)
        {
            if (!ModelState.IsValid)
            {
                return View(updateItem);
            }
            var item = _context.Items.FirstOrDefault(s => s.ItemId == updateItem.ItemId);
            if (item == null)
            {
                return NotFound();
            }
            if (updateItem.ItemImage!=null)
            {
                string uniqueFileName = null;

                // Delete old image if it exists
                if (!string.IsNullOrEmpty(item.ItemImageName) && item.ItemImageName != "item_default.png")
                {
                    string oldImagePath = Path.Combine(_environment.WebRootPath, "img/items", item.ItemImageName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                // Set the uploads folder path
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/items");
                // Ensure the folder exists
                Directory.CreateDirectory(uploadsFolder);
                // Generate a unique file name to avoid collisions
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(updateItem.ItemImage.FileName);
                // Combine folder path and file name
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Copy the uploaded file to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await updateItem.ItemImage.CopyToAsync(fileStream);
                }
                item.ItemImageName = uniqueFileName;
            }            
            item.ItemName = updateItem.ItemName;
            item.ItemPrice = updateItem.ItemPrice;
            item.ItemQuantity = updateItem.ItemQuantity;
            item.ItemChangedPrice = updateItem.ItemChangedPrice;
            item.Discount_rate = updateItem.Discount_rate;
            item.Discount_price = updateItem.Discount_price;
            item.ItemUpdatedDate = DateTime.Now;            
            _context.Items.Update(item);
            _context.SaveChanges();
            return RedirectToAction("Owner_Item_List", "Shop");
        }
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateItem()
        {
            var itemid = _context.Items.Max(s => s.ItemId);
            int newitemid = itemid + 1;
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            var view = new SingleItemViewModel
            {
                ItemId = newitemid,
                Shop_Id=user.ShopId
            };
            return View(view);
        }
        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> CreateItem(SingleItemViewModel item)
        {
            if (ModelState.IsValid)
            {                
                string uniqueFileName = null;
                // Check if an image is uploaded
                if (item.ItemImage != null)
                {
                    // Set the uploads folder path
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/items");

                    // Ensure the folder exists
                    Directory.CreateDirectory(uploadsFolder);

                    // Generate a unique file name to avoid collisions
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(item.ItemImage.FileName);

                    // Combine folder path and file name
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Copy the uploaded file to the specified path
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.ItemImage.CopyToAsync(fileStream);
                    }
                }
                var itemid = _context.Items.Max(s => s.ItemId);
                int newitem = itemid + 1;            
                var additem = new ItemModel()
                {
                    ItemId=newitem,
                    ItemName=item.ItemName,
                    ItemPrice=item.ItemPrice,
                    ItemQuantity=item.ItemQuantity,
                    ItemCreatedDate=DateTime.Now,
                    ItemUpdatedDate=DateTime.Now,
                    ItemChangedPrice=item.ItemPrice,
                    Shop_Id=item.Shop_Id,
                    Discount_rate=0,
                    Discount_price=0,
                    ItemImageName=uniqueFileName
                };
                _context.Items.Add(additem);
                _context.SaveChanges();
                return RedirectToAction("Owner_item_List", "Shop");
            }
            return View(item);
        }        
        [HttpPost]
        [ActionName("DeleteItem")]
        public IActionResult DeleteItem(int itemid)
        {
            var item = _context.Items.FirstOrDefault(s => s.ItemId == itemid);
            if(item == null)
            {
                return NotFound();
            }
            _context.Items.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Owner_Item_List", "Shop");
        }
    }
}
