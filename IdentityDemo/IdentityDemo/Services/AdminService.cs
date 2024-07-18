using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Repositories;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Services
{
    public class AdminService:IAdminService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IItemService _itemService;
        private readonly IOrderService _orderService;
        private readonly IShopRepository _shopRepository;
        private readonly IShopService _shopService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountService _accountService;
        private readonly IAccountRepository _accountRepository;

        public AdminService( IShopRepository shopRepository, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,IOrderService orderService,IShopService shopService,
            IItemService itemService,IAccountService accountService,ICategoryRepository categoryRepository,IAccountRepository accountRepository)
        {
            _shopService = shopService;
            _itemService = itemService;
            _orderService = orderService;
            _shopRepository = shopRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
            
        }
        public async Task<TableCountsViewModel> GetAdminDashboardDataAsync()
        {
            var orders = await _orderService.GetAllOrders();

            var orderCountsByDay = orders.GroupBy(o => o.OrderDate.Date)
                                         .Select(g => new { Date = g.Key, Count = g.Count() })
                                         .OrderBy(d => d.Date)
                                         .ToList();

            var labels = orderCountsByDay.Select(d => d.Date.ToString("yyyy-MM-dd")).ToArray();
            var data = orderCountsByDay.Select(d => d.Count).ToArray();

            //var normalCount = await _userRepository.GetUserCountByRoleAsync("User");
            //var ownerCount = await _userRepository.GetUserCountByRoleAsync("Owner");
            var userCount = _accountService.GetAllUser().Count();
            var orderCount = orders.Count();
            var shopCount = await _shopService.ShopCount();
            var itemCount = await _itemService.AllItemCount();

            return new TableCountsViewModel
            {
                UserCount = userCount,
                OrderCount = orderCount,
                ShopCount = shopCount,
                ItemCount = itemCount,
                NormalCount = 0,//test for pie chart
                OwnerCount = 0,//test fro pie chart
                OrderData = orders, 
                Labels = labels,
                Datas = data
            };
        }
        public async Task<List<CategoryModel>> GetCategoriesList()
        {
            var list= await _categoryRepository.GetAllCategories();
            return list;
        }
        public async Task AddCategoryAsync(CategoryModel category)
        {
            await _categoryRepository.AddCategoryAsync(category);
        }
        public async Task<List<ApplicationUser>> GetForgotPasswordUsersAsync()
        {
            var users = _accountService.GetAllUser();
            var fogotusers = users.Where(u => u.Forgot == 1).ToList();
            return fogotusers;
        }
        

        public async Task<List<ShopViewModel>> GetShopViewModelsAsync()
        {
            var shops = _shopRepository.GetAllShopsAsync();
            var users = _accountService.GetAllUser();

            return (from shop in shops
                    join user in users on shop.ShopId equals user.ShopId
                    select new ShopViewModel
                    {
                        ShopId = shop.ShopId,
                        ShopName = shop.ShopName,
                        ShopPhone = shop.ShopPhone,
                        ShopEmail = shop.ShopEmail,
                        ShopAddress = shop.ShopAddress,
                        ShopDescription = shop.ShopDescription,
                        ShopOwnerName = user.UserName
                    }).ToList();
        }      

        public async Task AdminUpdateUserAsync(UpdateUserViewModel model)
        {
            var user = await _accountService.GetUserByIdAsync(model.Id);
            if (model.Role == "Owner")
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, "Owner");

                if (user.ShopId == 0)
                {
                    int newShopId = await _shopRepository.GetNewShopIdAsync();
                    var shop = new ShopModel
                    {
                        ShopId = newShopId,
                        ShopName = "Shopname",
                        ShopPhone = user.PhoneNumber,
                        ShopDescription = "nice shop",
                        ShopAddress = " ",
                        ShopEmail = user.Email,
                        ProfileImagePath = "shop_default.png",
                        Is_confirm=0,
                    };
                    user.ShopId = newShopId;
                    await _shopRepository.AddShopAsync(shop);
                }
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, "Owner");
                await _userManager.AddToRoleAsync(user, "User");
            }

            user.Role = model.Role;
            await _accountRepository.UpdateNewUserAsync(user);
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _accountService.GetUserByIdAsync(userId);            
            user.Deleted = 1;
            await _accountRepository.DeleteUser(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string userId)
        {
            var user =await _accountService.GetUserByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, "Password123!");
            if (resetResult.Succeeded)
            {
                user.Forgot = 0;
                await _accountRepository.UpdateNewUserAsync(user);
            }
            return resetResult;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId,UpdateUserViewModel model)
        {
            var user = await _accountService.GetUserByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
            }
            return result;
        }
    }
}
