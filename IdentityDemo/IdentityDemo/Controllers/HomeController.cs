using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace IdentityDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IItemService _itemService;
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger,AppDbContext context,IItemService itemService,UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _itemService = itemService;
            _logger = logger;
            _context = context;
        }
        public ActionResult LoginError() => View();
        public ActionResult Show_error_loading() => View();
        public ActionResult Show_email_error_loading() => View();
        public async Task<IActionResult> Test64() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Role != "Owner") {
                return RedirectToAction("Login", "Account");
            }

            var view =await  _itemService.getSingleItemViewModelAsync(user.ShopId);

            return View(view);
        }
        public IActionResult About() => View();
        public IActionResult Index() => View();        
        public IActionResult Shop()=>View();
        public IActionResult Privacy=> View();
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
