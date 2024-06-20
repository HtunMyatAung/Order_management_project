using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
            var item = _context.Items.Where(s => s.Shop_Id == user.ShopId).ToList();
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult UpdateItem(int itemid)
        {
            var item = _context.Items.FirstOrDefault(s => s.ItemId == itemid);

            if (item== null)
            {
                return NotFound();
            }
            /*var updateitemviewmodel = new UpdateItemViewModel
            {
                ItemId = itemid,
                Shop_Id=item.Shop_Id,
                ItemName=item.ItemName,
                ItemPrice=item.ItemPrice,
                ItemQuantity=item.ItemQuantity,
                Discount_price=item.Discount_price,
                Discount_rate=item.Discount_rate,
                ItemChangedPrice=item.ItemChangedPrice,                
                ItemCreatedDate=item.ItemCreatedDate,
                ItemUpdatedDate=item.ItemUpdatedDate,
                
            };*/
            return View(item);
        }

        [HttpPost]
        public IActionResult UpdateItem(ItemModel updateItem)
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

        [HttpGet]
        public IActionResult Item_detail(int itemid)
        {
            var std = _context.Items.FirstOrDefault(s => s.ItemId == itemid);

            if (std == null)
            {
                return NotFound();
            }
            return View(std);
        }
        [Authorize(Roles = "Owner")]
        public IActionResult CreateItem()
        {
            var itemid = _context.Items.Max(s => s.ItemId);
            int newitemid = itemid + 1;
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            var view = new ItemModel
            {
                ItemId = newitemid,
                Shop_Id=user.ShopId
            };
            return View(view);
        }

        [HttpPost]
        public IActionResult CreateItem(ItemModel item)
        {
            if (ModelState.IsValid)
            {
                var itemid = _context.Items.Max(s => s.ItemId);
                int newitem = itemid + 1;
                item.ItemCreatedDate = DateTime.Now;
                item.ItemUpdatedDate = DateTime.Now;
                item.ItemId = newitem;
                item.ItemChangedPrice = item.ItemPrice;
                item.Discount_rate = 0;
                item.Discount_price = 0;
                _context.Items.Add(item);
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
            return RedirectToAction("Index", "Item");
        }
    }
}
