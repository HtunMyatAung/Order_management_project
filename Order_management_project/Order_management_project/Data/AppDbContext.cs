using Microsoft.EntityFrameworkCore;

namespace Order_management_project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<>
    }
}
