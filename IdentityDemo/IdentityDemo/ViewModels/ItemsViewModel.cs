using IdentityDemo.Models;

namespace IdentityDemo.ViewModels
{
    public class ItemsViewModel
    {
        
        public ShopModel Shop { get; set; }
        public List<ShopModel>? Shops { get; set; }
        public List<ItemModel>? Items { get; set; }
        public List<string>? Categories { get; set; }
        //public DateTime OrderDate { get; set; }
    }
}
