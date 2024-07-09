using IdentityDemo.Models;

namespace IdentityDemo.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<ApplicationUser> GetUsers();
        ApplicationUser GetUserById(string userId);
        Task UpdateUser(ApplicationUser user);
        Task DeleteUser(ApplicationUser user);
        Task<int> GetUserCountByRoleAsync(string role);
    }
}
