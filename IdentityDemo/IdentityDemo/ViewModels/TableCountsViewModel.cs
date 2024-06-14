using IdentityDemo.Models;

namespace IdentityDemo.ViewModels
{
    public class TableCountsViewModel
    {
        public int? UserCount { get; set; }
        public int? OrderCount { get; set; }
        public int? ShopCount { get; set; }
        public int? ItemCount { get; set; }
        public int? NormalCount { get; set; }
        public int? OwnerCount { get; set; }
        public string[] Labels{ get; set; }
        public int[] Datas{ get; set; }
        public List<OrderModel> OrderData { get; set; }
    }
}
