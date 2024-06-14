using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IdentityDemo.Models
{
    [Table("order")]
    public class OrderModel
    {
        [Required]
        [Key]
        [Column("id")]
        public int OrderID { get; set; }
        [Required]
        [Column("date")]
        public DateTime OrderDate { get; set; }
        [Column("total_price")]
        public decimal OrderPrice { get; set; }
        [Column("shop_id")]
        public int Shop_Id { get; set; }
        [Column("user_id")]
        public string User_Id { get; set; }

    }
}
