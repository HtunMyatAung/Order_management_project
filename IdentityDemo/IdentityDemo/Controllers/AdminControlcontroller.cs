using IdentityDemo.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.Controllers
{
    public class AdminControlcontroller : Controller
    {
        private readonly AppDbContext _context;
        public AdminControlcontroller(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Admin_shop_list()
        {
            var shop = _context.Shops.ToList();
            return View(shop);
        }
        public IActionResult Admin_forgot_list()
        {
            var forgots = _context.Users.Where(u=>u.Forgot==0). ToList();
            return View(forgots);
        }
        public IActionResult Admin_user_list()
        {
            var users = _context.Users.Where(u => u.Role != null && u.Role == "User").ToList();
            return View(users);
        }
        public IActionResult Admin_owner_list()
        {
            var owner = _context.Users.Where(u => u.Role != null && u.Role == "ShopOwner"). ToList();
            return View(owner);
        }
    }
}
