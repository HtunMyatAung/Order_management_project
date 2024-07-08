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

namespace IdentityDemo.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    public class AccountController_old : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;
        // private readonly IEmailSender _emailSender; // Implement an email sender service
        public AccountController_old(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,AppDbContext context, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment,IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _environment = environment;
            _emailService = emailService;
            //_emailSender = emailSender;
        }
        public IActionResult SendOTP(){return View();}
        [HttpGet]
        public IActionResult User_change_password()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Confirm_register(){return View();}
        [HttpPost]
        public async Task<IActionResult> SendOTP(string email)
        {
            string toEmail = email;
            string subject = "Verify your new uab zone account";
            var otp_code = GenerateOTP();
            //
            //TempData["EmailOTPCode"] = otp_code;
            var htmlText = $@"
   <!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Verify Your New Amazon Account</title>
    <style>
        /* Ensure styles are inline for better email client compatibility */
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            background-color: #f5f5f5;
            padding: 20px;
        }}

        .container {{
            max-width: 600px;
            margin: auto;
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}

        .otp-section {{
            background: #f0f0f0;
            padding: 10px;
            border-radius: 4px;
            margin-bottom: 20px;
        }}

        .otp-code {{
            font-size: 24px;
            font-weight: bold;
            color: #007bff;
            margin-bottom: 10px;
        }}

        .info-text {{
            margin-bottom: 10px;
        }}
        .note {{
            font-size: 14px;
            color: #777;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"">
        <tr>
            <td style=""padding: 20px;"">
                <div class=""container"">
                    <h2 style=""text-align: center;"">Verify Your New uab zone Account</h2>
                    <div class=""otp-section"">
                        <p style=""text-align: center;"">To verify your email address, please use the following One Time Password (OTP):</p>
                        <p class=""otp-code"" style=""text-align: center;"">{otp_code}</p>
                        <p class=""info-text"" style=""text-align: center;"">Do not share this OTP with anyone.uab zone takes your account security very seriously. uab zone Customer Service will never ask you to disclose or verify your uab zone password, OTP, credit card, or banking account number. If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to uab zone for investigation.</p>
                    </div>
                    <p style=""text-align: center;"">Thank you for shopping with us! We hope to see you again soon.</p>
                    
                </div>
            </td>
        </tr>
    </table>
</body>
</html>

";
            await _emailService.SendEmailAsync(toEmail, subject, htmlText);
            return RedirectToAction("Confirm_register", "Account");
        } 
        public async Task<IActionResult> Save_register()
        {
           
            // Retrieve rest of RegisterViewModel from session           
            var model = HttpContext.Session.GetObject<RegisterViewModel>("Registerviewmodel");
            Console.WriteLine($"Retrieved RegisterViewModel: {model}");

            

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            //Console.WriteLine(result.Succeeded);
            if (result.Succeeded)
            {
                user.Role = "User";
                user.PhoneNumber = model.PhoneNumber;
                user.PhoneNumberConfirmed = true;
                user.EmailConfirmed = true;
                user.Address = model.Address;
                user.Forgot = 0;
                user.ShopId = 0;
                user.UserImageName = "male_default.png";
                //user.UserImageName = "male_default.png";
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    // Log or handle the exception
                    Console.WriteLine("Error saving changes: " + ex.Message);
                    Console.WriteLine("Inner exception: " + ex.InnerException?.Message);
                    throw; // rethrow or handle the exception as needed
                }

                await _userManager.UpdateAsync(user);
                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                var adduserole = await _userManager.AddToRoleAsync(user, "User");
                // Redirect to appropriate page
                // Clear session after use
                HttpContext.Session.Remove("RegisterViewModel");
                return RedirectToAction("User_profile", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            // Clear session after use
            HttpContext.Session.Remove("RegisterViewModel");
            return View();
        }

        public IActionResult Admin_contact()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendChangePasswordEmail(string email)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(email);
                Console.WriteLine(user == null);
                if (user!=null) 
                {
                    
                    string toEmail = email;
                    string subject = "Reset password link";
                    string text= @"<a href=""https://localhost:7030/Account/UpdatePassword"" target=""_blank"">Reset Password link</a>";
                    await _emailService.SendEmailAsync(toEmail, subject, text);
                    user.Forgot = 1;
                    await _userManager.UpdateAsync(user);
                }
                else
                {                  
                    TempData["message"] = "Invalid email";
                    
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Show_error_loading", "Home");
            }
        }
        //[HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }
        // Helper method to generate 6-digit OTP
        private string GenerateOTP()
        {
            Random random = new Random();
            int otpNumber = random.Next(100000, 999999); // Generate a random 6-digit number
            return otpNumber.ToString("D6"); // Format as a 6-digit string
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                // Check if email is already taken
                var existingEmailUser = await _userManager.FindByEmailAsync(model.Email);
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
                string toEmail = model.Email;
                string subject = "Verify your new uab zone account";
                Random random = new Random();
                var otp_code = random.Next(100000, 999999).ToString();
                var otpmodel = new OTPViewModel { OTP = otp_code };
                TempData["Model"] = JsonConvert.SerializeObject(otpmodel);
                TempData["otpcode"] = otp_code;
                //ViewBag.Emailotpcode = otp_code;
                Console.WriteLine("otpppppppppppppppp " + otp_code );
                var htmlText = $@"
   <!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Verify Your New Amazon Account</title>
    <style>
        /* Ensure styles are inline for better email client compatibility */
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            background-color: #f5f5f5;
            padding: 20px;
        }}

        .container {{
            max-width: 600px;
            margin: auto;
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}

        .otp-section {{
            background: #f0f0f0;
            padding: 10px;
            border-radius: 4px;
            margin-bottom: 20px;
        }}

        .otp-code {{
            font-size: 24px;
            font-weight: bold;
            color: #007bff;
            margin-bottom: 10px;
        }}

        .info-text {{
            margin-bottom: 10px;
        }}
        .note {{
            font-size: 14px;
            color: #777;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"">
        <tr>
            <td style=""padding: 20px;"">
                <div class=""container"">
                    <h2 style=""text-align: center;"">Verify Your New uab zone Account</h2>
                    <div class=""otp-section"">
                        <p style=""text-align: center;"">To verify your email address, please use the following One Time Password (OTP):</p>
                        <p class=""otp-code"" style=""text-align: center;"">{otp_code}</p>
                        <p class=""info-text"" style=""text-align: center;"">Do not share this OTP with anyone.uab zone takes your account security very seriously. uab zone Customer Service will never ask you to disclose or verify your uab zone password, OTP, credit card, or banking account number. If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to uab zone for investigation.</p>
                    </div>
                    <p style=""text-align: center;"">Thank you for shopping with us! We hope to see you again soon.</p>
                    
                </div>
            </td>
        </tr>
    </table>
</body>
</html>

";
                await _emailService.SendEmailAsync(toEmail, subject, htmlText);

                // Save the model in the session
                HttpContext.Session.SetObject("Registerviewmodel", model);

                return RedirectToAction("Confirm_register", "Account");


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
                    if (isuser.Role=="Admin")
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
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
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
        [HttpGet]public IActionResult UpdatePassword(){return View();}
        [HttpPost][ValidateAntiForgeryToken]
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
            if (user.Forgot == 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid user");
                return View(model);

            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password to a default password
            var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                     
            if (resetResult.Succeeded)
            {
                user.Forgot = 0;
                // Optionally sign the user in again to refresh security tokens
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index","Home");
            }
            else
            {
                foreach (var error in resetResult.Errors)
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
                return RedirectToAction("Show_error_loading","Home");
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
            string uniqueFileName = model.Id + "_" + Path.GetFileName(user.UserImageName);

            // Set the uploads folder path
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/users");

            // Delete the old image if it exists
            if (!string.IsNullOrEmpty(user.UserImageName))
            {
                string oldImagePath = Path.Combine(uploadsFolder, user.UserImageName);

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Ensure the folder exists
            Directory.CreateDirectory(uploadsFolder);

            // Combine folder path and updated file name
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Copy the uploaded file to the specified path
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.UserImagePath.CopyToAsync(fileStream);
            }   
            // Update user properties
            user.Role= model.Role;
            user.UserName=model.UserName;
            user.Email = model.Email;
            user.Address = model.Useraddress;
            user.PhoneNumber = model.UserPhone;
            user.UserImageName = uniqueFileName;
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
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var username = User.Identity.;
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest();
            }

            // Fetch orders for the logged-in owner
            var orders = await _context.Orders
                                 .Where(o => o.User_Id == user.Id)
                                 .ToListAsync();

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
                OrderCount = orders.Count,
                UserId = user.Id,
                UserEmail = user.Email,
                UserPhone = user.PhoneNumber,
                UserName = user.UserName,
                Address = user.Address,
                Orders = orderViewModels,
                //UserImageName = user.UserImageName
            };

            return View(profileViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> User_profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    return NotFound();
                }
                if(model.UserImage != null) {
                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(user.UserImageName) && user.UserImageName!= "male_default.png")
                    {
                        string oldImagePath = Path.Combine(_environment.WebRootPath, "img/users", user.UserImageName);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    // Set the uploads folder path
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/users");

                    // Ensure the folder exists
                    Directory.CreateDirectory(uploadsFolder);

                    // Generate a unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.UserImage.FileName);


                    // Combine folder path and updated file name
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Copy the uploaded file to the specified path
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UserImage.CopyToAsync(fileStream);
                    }

                    // Update user image name in the model
                    user.UserImageName = uniqueFileName;

                }


                // Update other user properties
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


    }
}
