using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IdentityDemo.Models
{
    [Table("item")]
    public class ItemModel
    {
        [Key]        
        [Column("id")]
        public int ItemId { get; set; }
        [Column("name")]
        public string ItemName { get; set; }
        [Column("price")]
        public decimal ItemPrice { get; set; }
        [Column("quantity")]
        public int ItemQuantity { get; set; }
        [Column("created_date")]
        public DateTime ItemCreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime ItemUpdatedDate { get; set; }
        [Column("changed_price")]
        public decimal ItemChangedPrice { get; set; }
        [Column("shop_id")]
        public int Shop_Id { get; set; }
        [Column("discount_rate")]
        public int Discount_rate { get; set; }
        [Column("discount_price")]
        public decimal Discount_price { get; set; }
        [Column("item_image")]
        public string ItemImageName { get; set; }
        [Column("category")]
        public string Category { get; set; }
    }
}
