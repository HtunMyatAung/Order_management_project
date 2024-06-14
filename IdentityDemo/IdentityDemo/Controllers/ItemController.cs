using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace IdentityDemo.Controllers
{
    public class ItemController : Controller
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
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
            var itembyshopid = new ItemsViewModel
            {
                Shop = shop,
                Items = items
            };
            return View(itembyshopid);
        }
        public IActionResult SingleItemView(int itemid)
        {
            var item= _context.Items.FirstOrDefault(s=>s.ItemId == itemid);
            return View(item);
        }
        public IActionResult Index()
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

        public IActionResult UpdateItem(int itemid)
        {
            var std = _context.Items.FirstOrDefault(s => s.ItemId == itemid);

            if (std == null)
            {
                return NotFound();
            }
            return View(std);
        }

        [HttpPost]
        public IActionResult UpdateItem(ItemModel item)
        {

            if (ModelState.IsValid)
            {
                var std = _context.Items.FirstOrDefault(s => s.ItemId == item.ItemId);
                if (std == null)
                {
                    return NotFound();
                }
                std.ItemName = item.ItemName;
                std.ItemPrice = item.ItemPrice;
                std.ItemQuantity = item.ItemQuantity;
                std.ItemChangedPrice = item.ItemChangedPrice;
                
                std.ItemUpdatedDate = item.ItemUpdatedDate;
                _context.Items.Update(std);
                _context.SaveChanges();
                return RedirectToAction("Index", "Item");
            }
            return View(item);
        }
        [HttpGet]
        public IActionResult DetailItem(int itemid)
        {
            var std = _context.Items.FirstOrDefault(s => s.ItemId == itemid);

            if (std == null)
            {
                return NotFound();
            }
            return View(std);
        }

        public IActionResult CreateItem()
        {
            var std = _context.Items.Max(s => s.ItemId);
            int stdd = std + 1;
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            var view = new ItemModel
            {
                ItemId = stdd,
                Shop_Id=user.ShopId
            };
            return View(view);
        }

        [HttpPost]
        public IActionResult CreateItem(ItemModel item)
        {
            if (ModelState.IsValid)
            {
                var std = _context.Items.Max(s => s.ItemId);
                int stdd = std + 1;
                item.ItemCreatedDate = DateTime.Now;
                item.ItemId = stdd;
                _context.Items.Add(item);
                _context.SaveChanges();
                return RedirectToAction("Index", "Item");
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
            return RedirectToAction("Index", "Item");
        }
    }
}
