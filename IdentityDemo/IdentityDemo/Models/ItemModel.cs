using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IdentityDemo.Models
{
    [Table("item")]
    public class ItemModel
    {
        [Key]
        [Required]
        [Column("id")]
        public int ItemId { get; set; }
        [Required]
        [Column("name")]
        public string ItemName { get; set; }
        [Required]
        [Column("price")]
        public decimal ItemPrice { get; set; }
        [Required]
        [Column("quantity")]
        public int ItemQuantity { get; set; }
        [Required]
        [Column("created_date")]
        public DateTime ItemCreatedDate { get; set; }
        [Required]
        [Column("updated_date")]
        public DateTime ItemUpdatedDate { get; set; }
        [Required]
        [Column("changed_price")]
        public decimal ItemChangedPrice { get; set; }
        [Column("shop_id")]
        public int? Shop_Id { get; set; }
        //public ShopModel Shop { get; set; }
    }
}
