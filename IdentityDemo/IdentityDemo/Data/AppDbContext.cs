using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<ShopModel> Shops { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<ItemModel> Items { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        {
            Database.EnsureCreated();
        }
       
    }
}
