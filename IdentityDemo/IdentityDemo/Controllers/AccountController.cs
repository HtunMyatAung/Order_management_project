using IdentityDemo.Data;
using IdentityDemo.Filters;
using IdentityDemo.Models;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using IdentityDemo.Extensions;
using Org.BouncyCastle.Bcpg;
using System.Web.Helpers;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;
using MySqlX.XDevAPI;
using static System.Net.WebRequestMethods;
using System.Security.Claims;
using IdentityDemo.Repositories;

namespace IdentityDemo.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    public class Accountcontroller : Controller
    {
        public readonly IAccountRepository _accountRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;
        public readonly IAccountService _accountService;
        public Accountcontroller(IAccountService accountService,UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager, AppDbContext context, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _environment = environment;
            _emailService = emailService;
            _accountService = accountService;
        }
        public IActionResult SendOTP() => View();
        [HttpGet]
        public IActionResult User_change_password()=> View();
        [HttpGet]
        public IActionResult Confirm_register() => View();
        [HttpPost]
        public async Task<IActionResult> SendOTP(string email)
        {
            try
            {
                _accountService.SendOTP(email);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Show_email_error_loading", "Home");
            }
            
            return RedirectToAction("Confirm_register", "Account");
        }
        public async Task<IActionResult> Save_register()
        {
            var model = HttpContext.Session.GetObject<RegisterViewModel>("Registerviewmodel");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.CreateNewUser(model);

            // Clear session after use
            HttpContext.Session.Remove("RegisterViewModel");

            if (result)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Show_error_loading","Home");
            }
            
        }        
        [HttpPost]
        public async Task<IActionResult> SendChangePasswordEmail(string email)
        {
            try
            {
                var user = await _accountService.FindUserByEmailAsync(email);
                Console.WriteLine(user.UserName);
                if (user!=null) 
                {
                    _accountService.ChangePasswordSendEmail(user.Email);
                    _accountService.UpdateUserAsync1(user);
                }
                else
                {
                    TempData["message"] = "Invalid email";
                    Console.WriteLine("********************error***********");
                    
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Show_error_loading", "Home");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Register() => View();
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {            
            if (ModelState.IsValid)
            {
                // Check if email is already taken
                var existingEmailUser = await _accountService.FindUserByEmailAsync(model.Email);
                Console.WriteLine(existingEmailUser);
                if (existingEmailUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email is already taken.");
                    return View(model);
                }
                // Check if username is already taken
                var existingUsernameUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUsernameUser != null && existingUsernameUser is ApplicationUser)
                {
                    ModelState.AddModelError(string.Empty, "Username is already taken.");
                    return View(model);
                }
                Random random = new Random();
                var otp_code = random.Next(100000, 999999).ToString();
                var otpmodel = new OTPViewModel { OTP = otp_code };
                TempData["Model"] = JsonConvert.SerializeObject(otpmodel);
                TempData["otpcode"] = otp_code;
                try
                {
                    await _accountService.SendRegisterConfirmEmail(model.Email, otp_code);
                    // Save the model in the session
                    HttpContext.Session.SetObject("Registerviewmodel", model);
                    return RedirectToAction("Confirm_register", "Account");
                }
                catch (Exception ex) {
                    return RedirectToAction("Show_email_error_loading", "Home");
                }
            }
            // If registration fails, return the registration view with validation errors
            return View(model);
        }
        [HttpGet]
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                var isuser = await _userManager.FindByNameAsync(model.UserName);
                if (result.Succeeded && isuser != null && isuser.Deleted!=1)
                {
                    // Retrieve the current user
                    ApplicationUser currentUser = await _userManager.GetUserAsync(User);

                    // Pass the user data to the view
                    ViewBag.CurrentUser = currentUser;
                    if (isuser.Role=="Admin")
                    {
                        return RedirectToAction("Admin_dashboard", "AdminControl");
                    }
                    else if (isuser.Role == "Owner")
                    {
                        return RedirectToAction("Owner_dashboard", "Shop");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (result.IsLockedOut)
                {
                    return RedirectToAction("LoginError", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            ModelState.AddModelError(string.Empty, "Please enter valid data");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]public IActionResult UpdatePassword(){return View();}
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _accountService.FindUserByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            if (user.Forgot == 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid user");
                return View(model);
            }

            var resetResult = await _accountService.ResetPasswordAsync(user, model.NewPassword);

            if (resetResult)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to reset password.");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateUser()
        {
            var viewModel = _accountService.GetUpdateUserAsync(User);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _accountService.GetUserByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            
            bool updateResult = await _accountService.UpdateUserAsync(user, model);

            
            if (updateResult)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed To Update User");
                return View(model);
            }
        }        
        [HttpGet]
        public async Task<IActionResult> User_profile()
        {
            try
            {
                var profileViewModel = await _accountService.GetUserProfileAsync(User);

                return View(profileViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving user profile.");
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> User_profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _accountService.GetUserByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Handle user image upload
            if (model.UserImage != null)
            {
                // Save user image and update user's UserImageName property
                string newImageName = await _accountService.SaveUserImageAsync(model.UserId, model.UserImage);

                if (newImageName == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to save user image.");
                    return View(model);
                }
            }

            // Update user profile
            bool updateResult = await _accountService.UpdateUserProfileAsync(user, model);

            if (updateResult)
            {
                return RedirectToAction("User_profile", "Account");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to update user profile.");
                return View(model);
            }
        }
    }
}
