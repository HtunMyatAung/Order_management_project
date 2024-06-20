using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityDemo.Models
{
    [Table("orderdetail")]
    public class OrderDetailModel
    {
        [Key]
        [Column("id")]
        public int OrderDetailId { get; set; }
        [Column("quantity")]
        public int Item_quantity { get; set; }
        [Column("initial_price")]
        public decimal Initial_price { get; set; }
        [Column("final_price")]
        public decimal Final_price { get; set;}
        [Column("item_id")]
        public int ItemId { get; set; }
        [Column("order_id")]
        public int OrderId { get; set; }

    }
}
