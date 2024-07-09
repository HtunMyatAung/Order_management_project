using IdentityDemo.Data;
using IdentityDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Repositories
{
    public class ActionRepository:IActionRepository
    {
        private readonly AppDbContext _context;
        public ActionRepository(AppDbContext context)
        {

            _context = context;
        }

        public async Task Add(ActionLog log)
        {
           _context.ActionLogs.Add(log);
           _context.SaveChanges();
        }
    }
}
