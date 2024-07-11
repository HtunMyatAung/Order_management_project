using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace IdentityDemo.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        private readonly ILogger<AppDbContext> _logger;
        public bool IsInitializedSuccessfully { get; private set; }
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<ShopModel> Shops { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<ItemModel> Items { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        {
            Database.EnsureCreated();
        }
       
    }
}
