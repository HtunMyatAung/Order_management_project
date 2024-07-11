using IdentityDemo.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityDemo.ViewModels
{
    public class SingleItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemQuantity { get; set; }
        public DateTime ItemCreatedDate { get; set; }
        public DateTime ItemUpdatedDate { get; set; }
        public decimal ItemChangedPrice { get; set; }
        public int Shop_Id { get; set; }
        public int Discount_rate { get; set; }
        public decimal Discount_price { get; set; }
        public IFormFile? ItemImage { get; set; } // Property for image upload
        public string Category { get; set; }
        public List<string>? Categories { get; set; }
    }
}
