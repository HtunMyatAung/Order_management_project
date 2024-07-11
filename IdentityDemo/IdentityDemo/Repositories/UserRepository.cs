using IdentityDemo.Data;
using IdentityDemo.Models;

namespace IdentityDemo.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ApplicationUser> GetUsers()
        {
            return _context.Users.Where(u=> u.Role != "Admin"&& u.Deleted!=1).ToList();
        }

        public ApplicationUser GetUserById(string userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        public async Task UpdateUser(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetUserCountByRoleAsync(string role)
        {
            return _context.Users.Where(u => u.Role == role).Count();
        }
    }
}
