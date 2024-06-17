using IdentityDemo.Models;

namespace IdentityDemo.ViewModels
{
    public class OwnerViewModel
    {
        public ShopModel ownershop { get; set; }
        public OrderModel ownerorder { get; set; }
        public ItemModel owneritem { get; set; }
    }
}
