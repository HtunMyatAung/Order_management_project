using IdentityDemo.Models;

namespace IdentityDemo.ViewModels
{
    public class InvoiceViewModel
    {
        public ApplicationUser User { get; set; }
        public int OrderId { get; set; }
        public ShopModel Shop { get; set; }
        public List<ItemModel> Items { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
