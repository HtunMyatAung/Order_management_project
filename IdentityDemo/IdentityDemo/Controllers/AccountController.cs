using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Controllers
{
    public class Accountcontroller : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        public Accountcontroller(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            Console.WriteLine(ModelState.IsValid);
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    
                    // If registering as a shop owner, create the shop profile
                    if (model.RegisterAs == "ShopOwner")
                    {
                        int newShopId = 0;
                        int maxShopId = _context.Shops.Any() ? _context.Shops.Max(s => s.ShopId) : 0;
                        newShopId = maxShopId + 1;
                        var shop = new ShopModel
                        {
                            ShopId= newShopId,
                            ShopName = model.ShopName,
                            ShopPhone = model.ShopPhone,
                            ShopDescription = model.ShopDescription,
                            ShopAddress=model.ShopAddress,
                            ShopEmail=model.Email,
                            // Add other shop properties as needed
                        };
                        
                        
                        user.Role = model.RegisterAs;
                        user.ShopId =newShopId;
                        // Save the shop profile
                        _context.Shops.Add(shop);
                        

                        // Associate the shop with the user, if needed
                        //user.ShopId = shop.Id;
                        
                    }
                    else
                    {
                        user.Role=model.RegisterAs;
                        
                    }
                    user.Address = model.Address;
                    user.Forgot = 0;
                    await _context.SaveChangesAsync();
                    await _userManager.UpdateAsync(user);
                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to appropriate page
                    return RedirectToAction("User_page", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If registration fails, return the registration view with validation errors
            return View(model);
        }
        

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Privacy", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Privacy", "Home");
        }
        public IActionResult User_page()
        {
            var model = new UserViewModel();
            if (User.Identity.IsAuthenticated)
            {
                model.Email = User.Identity.Name;
            }
            return View(model);
        }
    }
}
