using IdentityDemo.Models;

namespace IdentityDemo.Repositories
{
    public interface IActionRepository
    {
        Task Add(ActionLog log);
    }
}
