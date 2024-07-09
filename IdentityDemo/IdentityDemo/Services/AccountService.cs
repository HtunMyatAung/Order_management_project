using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Repositories;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Web.Mvc;
using System.Xml;

namespace IdentityDemo.Services
{
    public class AccountService : IAccountService
    {
        public readonly IAccountRepository _accountRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;
        public AccountService(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager, IAccountRepository accountRepository, AppDbContext context, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _environment = environment;
            _emailService = emailService;
            _accountRepository = accountRepository;
        }


        public async void SendOTP(string email)
        {
            string toEmail = email;
            string subject = "Verify your new uab zone account";
            var otp_code = GenerateOTP();
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
        }
        

        

        public async void SendRegisterConfirmEmail(string email,string otpcode)
        {
            string toEmail = email;
            string subject = "Verify your new uab zone account";
            
            //ViewBag.Emailotpcode = otp_code;
            Console.WriteLine("otpppppppppppppppp " + otpcode);
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
                        <p class=""otp-code"" style=""text-align: center;"">{otpcode}</p>
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
        }

        public async Task<bool> CreateNewUser(RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };

            var result = await _accountRepository.CreateNewUser(user, model.Password);

            if (result != null)
            {
                user.Role = "User";
                user.PhoneNumber = model.PhoneNumber;
                user.PhoneNumberConfirmed = true;
                user.EmailConfirmed = true;
                user.Address = model.Address;
                user.Forgot = 0;
                user.ShopId = 0;
                user.UserImageName = "male_default.png";

                try
                {
                    _accountRepository.UpdateNewUser(user);
                }
                catch (DbUpdateException ex)
                {
                    // Log or handle the exception
                    Console.WriteLine("Error saving changes: " + ex.Message);
                    Console.WriteLine("Inner exception: " + ex.InnerException?.Message);
                    throw; // rethrow or handle the exception as needed
                }

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Add user to role
                await _userManager.AddToRoleAsync(user, "User");

                return true;
            }
            else
            {
                /*foreach (var error in result.Errors)
                {
                    // Add model error to show in view
                    ModelState.AddModelError(string.Empty, error.Description);
                }*/
                return false;
            }
        }

        public void LoginRole(LoginViewModel model)
        {
            throw new NotImplementedException();
        }
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _accountRepository.GetUserByIdAsync(userId);
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user, UpdateUserViewModel model)
        {
            
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
            // Update user properties based on ViewModel
            user.Role = model.Role;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Address = model.Useraddress;
            user.PhoneNumber = model.UserPhone;
            user.UserImageName = uniqueFileName;
           

            // Perform additional business logic if needed

            // Update user in repository
            return await _accountRepository.UpdateUserAsync(user);
        }

        public async Task<ApplicationUser> FindUserByEmailAsync(string email)
        {
            return await _accountRepository.FindByEmailAsync1(email);
        }

        public async Task<bool> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _accountRepository.ResetPasswordAsync(user, token, newPassword);

            if (resetResult)
            {
                // Optionally update additional user properties or perform other actions
                user.Forgot = 0;
                await _userManager.UpdateAsync(user);
            }

            return resetResult;
        }

        public async Task<bool> UpdateUserProfileAsync(ApplicationUser user, ProfileViewModel model)
        {
            // Update user properties based on ViewModel
            user.UserName = model.UserName;
            user.PhoneNumber = model.UserPhone;
            user.Email = model.UserEmail;
            user.Address = model.Address;

            // Update user in repository
            return await _accountRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserImageAsync(string userId, string imageName)
        {
            return await _accountRepository.DeleteUserImageAsync(userId, imageName);
        }

        public async Task<string> SaveUserImageAsync(string userId, IFormFile imageFile)
        {
            return await _accountRepository.SaveUserImageAsync(userId, imageFile);
        }

        public async Task<ProfileViewModel> GetUserProfileAsync(ClaimsPrincipal user)
        {
            var applicationUser = await _userManager.GetUserAsync(user);
            if (applicationUser == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(user)}'.");
            }

            var orders = await _context.Orders
                             .Where(o => o.User_Id == applicationUser.Id)
                             .ToListAsync();

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
                UserId = applicationUser.Id,
                UserEmail = applicationUser.Email,
                UserPhone = applicationUser.PhoneNumber,
                UserName = applicationUser.UserName,
                Address = applicationUser.Address,
                Orders = orderViewModels,
            };

            return profileViewModel;
        }

        public  async Task<UpdateUserViewModel> GetUpdateUserAsync(ClaimsPrincipal user)
        {
            var applicationUser = await _accountRepository.GetUserAsync(user);
            if (applicationUser == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(user)}'.");
            }

            var viewModel = new UpdateUserViewModel
            {
                Role = applicationUser.Role,
                UserName = applicationUser.UserName,
                Email = applicationUser.Email,
                Useraddress = applicationUser.Address,
                UserPhone = applicationUser.PhoneNumber
                
            };

            return viewModel;
        }














        private string GenerateOTP()// Helper method to generate 6-digit OTP
        {
            Random random = new Random();
            int otpNumber = random.Next(100000, 999999); // Generate a random 6-digit number
            return otpNumber.ToString("D6"); // Format as a 6-digit string
        }

        public async void UpdateUserAsync1(ApplicationUser user)
        {
            user.Forgot = 1;
            _accountRepository.UpdateNewUser(user);
        }

        public async void ChangePasswordSendEmail(string email)
        {
            string toEmail = email;
            string subject = "Reset password link";

            string htmlText = @"<!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Reset Password</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    padding: 20px;
                }
                .container {
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }
                h1 {
                    color: #333333;
                    text-align: center;
                }
                p {
                    color: #555555;
                    font-size: 16px;
                    line-height: 1.6;
                }
                .button {
                    display: inline-block;
                    background-color: #007bff;
                    color: #ffffff;
                    text-decoration: none;
                    padding: 10px 20px;
                    border-radius: 4px;
                    margin-top: 20px;
                }
                .button:hover {
                    background-color: #0056b3;
                }
            </style>
        </head>
        <body>
            <div class=""container"">
                <h1>Reset Your Password</h1>
                <p>Dear User,</p>
                <p>To reset your password, please click the button below:</p>
                <a href=""http://10.235.151.97:5000/Account/UpdatePassword"" target=""_blank"">Reset Password link</a>
                <p>If you didn't request this change, you can safely ignore this email.</p>
                <p>Best regards,<br>uab Zone</p>
            </div>
        </body>
        </html>";

            // Assuming _emailService is properly injected
            //await _emailService.SendEmailAsync(toEmail, subject, text);
            await _emailService.SendEmailAsync(toEmail,subject, htmlText);
        }
    }
}
