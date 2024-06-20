using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;

namespace IdentityDemo.Controllers
{
    public class Accountcontroller : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        // private readonly IEmailSender _emailSender; // Implement an email sender service
        public Accountcontroller(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            //_emailSender = emailSender;
        }
        public IActionResult Admin_contact()
        {
            return View();
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
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {          
                    user.Role = "User";
                    user.PhoneNumber= model.PhoneNumber;
                    user.PhoneNumberConfirmed = true;
                    user.EmailConfirmed = true;
                    user.Address = model.Address;
                    user.Forgot = 0;
                    user.ShopId = 0;
                    await _context.SaveChangesAsync();
                    await _userManager.UpdateAsync(user);
                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var adduserole = await _userManager.AddToRoleAsync(user, "User");
                    // Redirect to appropriate page
                    return RedirectToAction("User_profile", "Account");
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
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                var isuser = await _userManager.FindByNameAsync(model.UserName);
                if (result.Succeeded && isuser!=null)
                {
                    // Retrieve the current user
                    ApplicationUser currentUser = await _userManager.GetUserAsync(User);

                    // Pass the user data to the view
                    ViewBag.CurrentUser = currentUser;
                    if (model.UserName=="admin@gmail.com" && isuser.Role=="Admin")
                    {
                        return RedirectToAction("Admin_dashboard", "AdminControl");
                    }
                    else if(isuser.Role== "Owner")
                    {
                        return RedirectToAction("Owner_dashboard", "Shop");
                    }
                    else { 
                        return RedirectToAction("Index", "Home");
                    }
                    
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
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
        [HttpGet]
        public IActionResult UpdatePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
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
                return RedirectToAction("Index","Home");
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

        public IActionResult PasswordChangeSuccess()
        {
            return RedirectToAction("Privacy", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle user not found error
                return NotFound();
            }

            var viewModel = new UpdateUserViewModel
            {
                Role=user.Role,
                UserName = user.UserName,
                Email = user.Email,
                Useraddress = user.Address,
                UserPhone = user.PhoneNumber
                // Populate other properties as needed
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If model state is not valid, return the view with validation errors
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle user not found error
                return NotFound();
            }

            // Update user properties
            user.Role= model.Role;
            user.UserName=model.UserName;
            user.Email = model.Email;
            user.Address = model.Useraddress;
            user.PhoneNumber = model.UserPhone;

            // Update other properties as needed

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Update successful
                return RedirectToAction("Index","Home");
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
        // GET: AdminControl/Admin_update_user_info/{userId}
        [HttpGet]
        public async Task<IActionResult> User_profile()
        {
            var user= await _userManager.GetUserAsync(User);       
            if (user == null)
            {
                return BadRequest();
            }
            // Fetch orders for the logged-in owner
            var orders = _context.Orders
                                 .Where(o => o.User_Id == user.Id)
                                 .ToList();

            // Create a list of OrderViewModel with User_Name
            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                OrderID = o.OrderID,
                OrderDate = o.OrderDate,
                OrderPrice = o.OrderPrice,                
                Shop_Name = _context.Shops.FirstOrDefault(u => u.ShopId == o.Shop_Id)?.ShopName
            }).ToList();
            var profileViewModel = new ProfileViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserPhone=user.PhoneNumber,
                UserName=user.UserName,
                Address=user.Address,
                Orders = orderViewModels
            };
            return View(profileViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> User_profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId); // Use FindByIdAsync to get the user

                if (user == null)
                {
                    return NotFound();
                }

                // Update user properties based on the model
                user.Id= model.UserId;
                user.UserName = model.UserName;
                user.PhoneNumber = model.UserPhone;
                user.Email = model.UserEmail;
                user.Address = model.Address;

                // Update user in UserManager and save changes
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    // Handle errors if update fails
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model); // Return the view with errors
                }

                // Redirect to GET action upon successful update
                return RedirectToAction("User_profile", "Account");
            }

            // If model state is not valid, return the view with validation errors
            return View(model);
        }

        /* [HttpGet]
         public IActionResult ForgotPassword()
         {
             return View();
         }

         [HttpPost]
         public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
         {
             if (ModelState.IsValid)
             {
                 var user = await _userManager.FindByEmailAsync(model.Email);
                 if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                 {
                     // Don't reveal that the user does not exist or is not confirmed
                     return RedirectToAction("ForgotPasswordConfirmation");
                 }

                 var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                 var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                 await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                     $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                 return RedirectToAction("ForgotPasswordConfirmation");
             }

             return View(model);
         }

         [HttpGet]
         public IActionResult ForgotPasswordConfirmation()
         {
             return View();
         }

         [HttpGet]
         public IActionResult ResetPassword(string code = null)
         {
             if (code == null)
             {
                 return BadRequest("A code must be supplied for password reset.");
             }

             return View(new ResetPasswordViewModel { Code = code });
         }

         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
         {
             if (!ModelState.IsValid)
             {
                 return View(model);
             }

             var user = await _userManager.FindByEmailAsync(model.Email);
             if (user == null)
             {
                 // Don't reveal that the user does not exist
                 return RedirectToAction("ResetPasswordConfirmation");
             }

             var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
             if (result.Succeeded)
             {
                 return RedirectToAction("ResetPasswordConfirmation");
             }

             foreach (var error in result.Errors)
             {
                 ModelState.AddModelError(string.Empty, error.Description);
             }
             return View(model);
         }

         [HttpGet]
         public IActionResult ResetPasswordConfirmation()
         {
             return View();
         }*/

    }
}
