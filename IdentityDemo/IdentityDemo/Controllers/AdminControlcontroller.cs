using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Repositories;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace IdentityDemo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminControlController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAdminService _adminService;
        private readonly IShopService _shopService;
        private readonly IActionRepository _actionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly ICategoryRepository _categoryRepository;
        public AdminControlController( IAdminService adminService,IShopService shopService,
            UserManager<ApplicationUser> userManager,IActionRepository actionRepository,
            SignInManager<ApplicationUser> signInManager,IAccountService accountService,AppDbContext context,ICategoryRepository categoryRepository)
        {
           
            _adminService = adminService;
            _shopService = shopService;
            _userManager = userManager;
            _actionRepository = actionRepository;
            _signInManager = signInManager;
            _accountService = accountService;
            _context = context;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index() => View();
        [HttpGet]
        public async Task<IActionResult> Admin_category_list(){
            var model = new CategoryListViewModel
            {
                List = await _adminService.GetCategoriesList(),
                NewCategory = new IdentityDemo.Models.CategoryModel()
            };
            return View(model);
        }
        [HttpPost]
        [ActionName("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int categoryid)
        {
            await _categoryRepository.DeleteCategoryAsync(categoryid);
            return RedirectToAction("Admin_category_list", "AdminControl");
        }

        [HttpPost]
        public async Task<IActionResult> Admin_add_category(string category_name)
        {
            var existingCategory = await _categoryRepository.GetCategoryByNameAsync(category_name);
            if (existingCategory != null)
            {
                TempData["error_category"] = "This category is already exsit";
                return RedirectToAction("Admin_category_List", "AdminControl");
            }
            var model = new CategoryModel()
            {
                
                Name = category_name,
                Item_count = 0
            };
            await _adminService.AddCategoryAsync(model);
            return RedirectToAction("Admin_category_List", "AdminControl"); 
            
        }

        public async Task<IActionResult> Admin_shop_list()
        {
            TempData["title"] = "Shop list";
            var shopViewModels = await _shopService.GetShopsNOwnersAsync();
            return View(shopViewModels);
        }

        public async Task<IActionResult> Admin_forgot_list()
        {
            TempData["title"] = "Forgot password list";
            var forgots = await _adminService.GetForgotPasswordUsersAsync();
            return View(forgots);
        }

        [HttpGet]
        public async Task<IActionResult> Admin_update_user_info(string userId)
        {
            TempData["title"] = "Update User information";
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            var model = await _accountService.GetUpdateUserViewModelAsync(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin_update_user_info(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await _adminService.AdminUpdateUserAsync(model);
            return RedirectToAction("Admin_user_list");
        }

        public async Task<IActionResult> Admin_dashboard()
        {
            TempData["title"] = "Dashboard";
            var viewModel = await _adminService.GetAdminDashboardDataAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Admin_user_list()
        {
            string error = string.Empty;
            var loginUserId = _userManager.GetUserId(User);
            string responseData = string.Empty;
            try
            {
                TempData["title"] = "User List";
                var users = _accountService.GetAllUser();
                responseData = JsonConvert.SerializeObject(users);

                return View(users);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                responseData = error;
                return RedirectToAction("Show_error_loading", "Home");
            }
            finally
            {
                // Log the action
                var log = new ActionLog
                {
                    UserId = loginUserId,
                    ActionName = "Admin_user_list",
                    ControllerName = "AdminControl",
                    Timestamp = DateTime.Now,
                    RequestData = null,
                    ResponseData = responseData,
                    LogStatus = string.IsNullOrEmpty(error) ? "INFO" : "ERROR"
                };
                _actionRepository.Add(log); 
                
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            await _adminService.DeleteUserAsync(userId);
            return RedirectToAction("Admin_user_list");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            TempData["title"] = "Reset password";
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            await _adminService.ResetPasswordAsync(userId);
            return RedirectToAction("Admin_forgot_list");
        }

        [HttpGet]
        public IActionResult Admin_change_password() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin_change_password(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = _userManager.GetUserId(User);
            var result=await _adminService.ChangePasswordAsync(userId,model);
            if (result.Succeeded)
            {

                return RedirectToAction("Admin_user_list");
            }
            return RedirectToAction("Show_error_loading", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
