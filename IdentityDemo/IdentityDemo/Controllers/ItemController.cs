using IdentityDemo.Models;
using IdentityDemo.Repositories;
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
        private readonly ICategoryRepository _categoryRepository;

        public ItemController(IItemService itemService, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager,ICategoryRepository categoryRepository)
        {
            _itemService = itemService;
            _environment = environment;
            _userManager = userManager;
            _categoryRepository = categoryRepository;

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
        [HttpGet]
        public async Task<IActionResult> CreateItem()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null && user.Role != "Owner")
            {
                return RedirectToAction("Login", "Account");
            }
            var viewModel = await _itemService.getSingleItemViewModelAsync(user.ShopId); // Ensure await here

            return View(viewModel); // Pass the resolved SingleItemViewModel to the view
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
                else
                {
                    uniqueFileName = "item_default.png";
                }
                //item.Categories = item.Categories;

                await _itemService.AddItemAsync(item, uniqueFileName);
                return RedirectToAction("Owner_item_List", "Shop");
            }
            
            item.Categories= await _categoryRepository.GetCategoryNamesAsync();

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
