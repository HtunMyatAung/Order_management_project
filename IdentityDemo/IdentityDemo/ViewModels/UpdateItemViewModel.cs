using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityDemo.ViewModels
{
    public class UpdateItemViewModel
    {
        public int ItemId { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]
        public decimal ItemPrice { get; set; }
        [Required]
        public int ItemQuantity { get; set; }
        [Required]
        public decimal ItemChangedPrice { get; set; }
        public int? Shop_Id { get; set; } // Nullable if not always needed
        public int? Discount_rate { get; set; } // Nullable if optional
        public decimal? Discount_price { get; set; } // Nullable if optional
        public DateTime? ItemCreatedDate { get; set; }
        public DateTime? ItemUpdatedDate { get; set; }
    }

}
