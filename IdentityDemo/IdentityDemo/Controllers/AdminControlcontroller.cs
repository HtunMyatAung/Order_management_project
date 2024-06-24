using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Controllers
{
    public class AdminControlcontroller : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AdminControlcontroller(AppDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Admin_shop_list()
        {
            var admin = User.Identity.Name;

            var shop = _context.Shops.ToList();
            return View(shop);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Admin_forgot_list()
        {
            var forgots = _context.Users.Where(u=>u.Forgot==1). ToList();
            return View(forgots);
        }
        // GET: AdminControl/Admin_update_user_info/{userId}
        [HttpGet]
        public async Task<IActionResult> Admin_update_user_info(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            //Console.WriteLine(user.Role);
            var model = new UpdateUserViewModel
            {
                Id = userId, // Add the user ID to the model
                UserName = user.UserName,
                Email = user.Email,
                UserPhone = user.PhoneNumber,
                Role = user.Role,
                Useraddress = user.Address
            };

            return View(model);
        }
        // Method to get default shop image as byte[]
        private byte[] GetDefaultShopImage()
        {
            // Example: Load default image file and convert to byte[]
            var imagePath = "wwwroot/img/shop/shop_default.png"; // Adjust path as per your project structure
            byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
            return imageData;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin_update_user_info(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If model state is not valid, return the view with validation errors
                return View(model);
            }
            
            var user = await _userManager.FindByIdAsync(model.Id); // Retrieve the user using the provided ID
            
            if (model.Role == "Owner")
            {
                // Remove the current role from the user
                var removeResult = await _userManager.RemoveFromRoleAsync(user, "User");
                // Add the new role to the user
                var addResult = await _userManager.AddToRoleAsync(user, "Owner");
            }
            else
            {
                // Remove the current role from the user
                var removeResult = await _userManager.RemoveFromRoleAsync(user, "Owner");
                // Add the new role to the user
                var addResult = await _userManager.AddToRoleAsync(user, "User");
            }
            if (model.Role=="Owner" && user.ShopId == 0)
            {

                int newShopId = 0;
                int maxShopId = _context.Shops.Any() ? _context.Shops.Max(s => s.ShopId) : 0;
                newShopId = maxShopId + 1;
                var shop = new ShopModel
                {
                    ShopId = newShopId,
                    ShopName ="Shopname",
                    ShopPhone = user.PhoneNumber,
                    ShopDescription = "nice shop",
                    ShopAddress =user.Address,
                    ShopEmail =user.Email,
                    //ShopImage= "Shop_default.png"
                    // Add other shop properties as needed
                };
                user.ShopId= newShopId;
                _context.Shops.Add(shop);
            }
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update user properties
            user.Role = model.Role;
            

            // Update other properties as needed

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Update successful
                return RedirectToAction("Admin_user_list", "AdminControl");
            }
            else
            {
                // Update failed, handle the error
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin_dashboard()
        {
            // Fetch orders from the database
            var orders = _context.Orders.ToList();
            

            // Prepare order data for the chart
            var orderData = orders.Select(o => new OrderModel
            {
                OrderDate = o.OrderDate,
                OrderPrice = o.OrderPrice
            })
            .OrderBy(o => o.OrderDate)
            .ToList();
            var orderCountsByDay = orders.GroupBy(o => o.OrderDate.Date)
                             .Select(g => new { Date = g.Key, Count = g.Count() })
                             .OrderBy(d => d.Date)
                             .ToList();
            //Console.WriteLine();
            //Console.WriteLine("haha" + orderCountsByDay==null);
            // Extract labels (dates) and data (order counts) for the chart
            var labels = orderCountsByDay.Select(d => d.Date.ToString("yyyy-MM-dd")).ToArray();
            var data = orderCountsByDay.Select(d => d.Count).ToArray();
            int normalCount = _context.Users.Where(u => u.Role != null && u.Role != "User").ToList().Count;
            int ownerCount = _context.Users.Where(u => u.Role != null && u.Role != "Owner").ToList().Count;
            // _context.Users.Where(u => u.Role != null && u.Role != "Admin").ToList();

            // Get the count of rows in the Users table
            int userCount = _context.Users.Count();

            // Get the count of rows in the Orders table
            int orderCount = _context.Orders.Count();

            // Get the count of rows in the Shops table
            int shopCount = _context.Shops.Count();
            int itemCount=_context.Items.Count();

            // Create ViewModel or use ViewBag to pass counts to the view
            var viewModel = new TableCountsViewModel
            {
                UserCount = userCount,
                OrderCount = orderCount,
                ShopCount = shopCount,
                ItemCount= itemCount,
                NormalCount = normalCount,
                OwnerCount= ownerCount,
                OrderData=orders,
                Labels=labels,
                Datas=data

                
            };
            Console.WriteLine("gg"+viewModel.OrderData+"hh");
            return View(viewModel);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Admin_user_list()
        {
            var users = _context.Users.Where(u => u.Role != null && u.Role != "Admin").ToList();
            return View(users);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Admin_owner_list()
        {
            var owner = _context.Users.Where(u => u.Role != null && u.Role == "ShopOwner"). ToList();
            return View(owner);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Admin_user_list","AdminControl"); // Redirect to a list view or appropriate page
            }

            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("Error"); // Or return to the appropriate view with the error messages
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Generate the password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password to a default password
            var resetResult = await _userManager.ResetPasswordAsync(user, token, "Password123!");

            if (resetResult.Succeeded)
            {
                // Optional: Inform the admin that the password reset was successful
                user.Forgot = 0;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Admin_forgot_list", "AdminControl");
            }
            else
            {
                // Handle errors
                foreach (var error in resetResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest("Failed to reset password.");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Admin_change_password()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin_change_password(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Console.WriteLine(User.Identity.Name);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                // Optionally sign the user in again to refresh security tokens
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Admin_user_list","AdminControl");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        private string GenerateDefaultPassword(string userId)
        {
            // You can implement any logic to generate a default password based on the user's ID
            // For example, you can concatenate the user's ID with a constant string or use any hashing algorithm
            return userId + "DefaultPassword";
        }
    }
}
