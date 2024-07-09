// ShopController.cs
using IdentityDemo.Models;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityDemo.Controllers
{
    public class ShopController : Controller
    {
        private readonly IShopService _shopService;
        private readonly IItemService _itemService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;
        public ShopController(IShopService shopService,IItemService itemService,UserManager<ApplicationUser> userManager,IOrderService orderService)
        {
            _shopService = shopService;
            _itemService = itemService;
            _userManager = userManager;
            _orderService = orderService;
            
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Owner_Item_List()
        {
            var user = await _userManager.GetUserAsync(User);
            var items =await _itemService.GetAllItemsByShopIdAsync(user.ShopId);
            if (items == null)
            {
                return NotFound();
            }
            return View(items);
        }

        public async Task<IActionResult> Shop_view(int shopid)
        {
            var shop = await _shopService.GetShopByIdAsync(shopid);
            var user=await _userManager.GetUserAsync(User);
            if (shopid != user.ShopId)
            {
                return RedirectToAction("Show_error_loading", "Home");
            }
            var items = await _itemService.GetItemNShopByShopIdAsync(shopid);
            
            return View(items);
        }

        public async Task<IActionResult> Owner_order_list()
        {
            var user= await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var orderViewModels = await _orderService.GetOrderNUserByShopIdAsync(user.ShopId);

            return View(orderViewModels);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet]
        public async Task<IActionResult> UpdateShop()
        {
            var user = await _userManager.GetUserAsync(User);
            var shop = await _shopService.GetShopByIdAsync(user.ShopId);
            var shopview = new ShopViewModel
            {
                ShopId = shop.ShopId,
                ShopName = shop.ShopName,
                ShopEmail = shop.ShopEmail,
                ShopAddress = shop.ShopAddress,
                ShopPhone = shop.ShopPhone,
                ShopDescription = shop.ShopDescription,
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
                var user= await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound();
                }

                await _shopService.UpdateShopAsync(shop);

                return RedirectToAction("Owner_dashboard", "Shop");
            }

            return View(shop);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteShop(int shopid)
        {
            var shop = await _shopService.GetShopByIdAsync(shopid);
            if (shop == null)
            {
                return NotFound();
            }

            return View(shop);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> DeleteShop(ShopViewModel shop)
        {
            if (ModelState.IsValid)
            {
                await _shopService.DeleteShopAsync(shop.ShopId);
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Owner_dashboard()
        {
            var user = await _userManager.GetUserAsync(User); 
            if (user == null || user.ShopId == null)
            {
                return NotFound();
            }

            var dashboard = await _shopService.GetOwnerDashboardAsync(user.Id);
            return View(dashboard);
        }
    }
}
