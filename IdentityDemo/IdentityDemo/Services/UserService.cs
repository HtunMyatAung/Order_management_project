using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Repositories;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;
        public UserService(IUserRepository userRepository,AppDbContext context,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,IEmailService emailService,IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _context = context;
            _userManager= userManager;
            _signInManager= signInManager;
            _emailService= emailService;
            _environment= webHostEnvironment;
        }
        public IEnumerable<ApplicationUser> GetAllUser()
        {
            return _userRepository.GetUsers();
        }
        public async Task<int> GetUserCount()
        {            
            return _userRepository.GetUsers().Count();
        }
        public async Task DeleteUserAsync(string userId)
        {
            var user =  _userRepository.GetUserById(userId);
            await _userRepository.DeleteUser(user);
        }
       
    }

}

