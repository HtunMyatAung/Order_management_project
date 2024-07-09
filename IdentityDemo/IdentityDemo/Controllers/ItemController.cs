using IdentityDemo.Models;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace IdentityDemo.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemController(IItemService itemService, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _itemService = itemService;
            _environment = environment;
            _userManager = userManager;
        }

        public async Task<IActionResult> HomePageItems()
        {
            var itemsViewModel = await _itemService.GetHomePageItemsAsync();
            return View(itemsViewModel);
        }

        public async Task<IActionResult> ItemByShopid(int shopid)
        {
            var itemsViewModel = await _itemService.GetItemNShopByShopIdAsync(shopid);
            if (!itemsViewModel.Items.Any())
            {
                TempData["items"] = "hello";
            }
            return View(itemsViewModel);
        }

        public async Task<IActionResult> UpdateItem(int itemid)
        {
            var user = await _userManager.GetUserAsync(User);
            var updateItemViewModel = await _itemService.GetItemForUpdateAsync(itemid, user.Id);
            if (updateItemViewModel == null)
            {
                return RedirectToAction("Show_error_loading", "Home");
            }
            return View(updateItemViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItem(SingleItemViewModel updateItem)
        {
            if (!ModelState.IsValid)
            {
                return View(updateItem);
            }

            string uniqueFileName = null;
            if (updateItem.ItemImage != null)
            {
                uniqueFileName = await ProcessUploadedFile(updateItem.ItemImage);
            }

            await _itemService.UpdateItemAsync(updateItem, uniqueFileName);
            return RedirectToAction("Owner_Item_List", "Shop");
        }

        [Authorize(Roles = "Owner")]
        public IActionResult CreateItem()
        {
            var username = User.Identity.Name;
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == username);
            var view = new SingleItemViewModel
            {
                Shop_Id = user.ShopId
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
                if (item.ItemImage != null)
                {
                    uniqueFileName = await ProcessUploadedFile(item.ItemImage);
                }

                await _itemService.AddItemAsync(item, uniqueFileName);
                return RedirectToAction("Owner_item_List", "Shop");
            }
            return View(item);
        }

        [HttpPost]
        [ActionName("DeleteItem")]
        public async Task<IActionResult> DeleteItem(int itemid)
        {
            await _itemService.DeleteItemAsync(itemid);
            return RedirectToAction("Owner_Item_List", "Shop");
        }

        private async Task<string> ProcessUploadedFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/items");
                Directory.CreateDirectory(uploadsFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
