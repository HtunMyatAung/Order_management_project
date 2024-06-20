using IdentityDemo.Models;

namespace IdentityDemo.ViewModels
{
    public class DashboardViewModel
    {
        public int? CustomerCount { get; set; }
        public int? OrderCount { get; set; }
        public int? ShopCount { get; set; }
        public int? ItemCount { get; set; }
        public string[] Labels { get; set; }
        public int[] Datas { get; set; }
        public List<OrderModel> OrderData { get; set; }
        public List<ItemModel> ItemData { get; set; }
        public List<ApplicationUser> UserData { get; set; }
    }
}
