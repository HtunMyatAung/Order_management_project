using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityDemo.Models
{
    [Table("shop")]
    public class ShopModel
    {
        [Key]
        [Column("id")]
        public int ShopId { get; set; }

        [Column("name")]
        public string ShopName { get; set; }

        [Column("description")]
        public string ShopDescription { get; set; }

        [Column("phone")]
        public string ShopPhone { get; set; }

        [Column("email")]
        public string ShopEmail { get; set; }

        [Column("address")]
        public string ShopAddress { get; set; }
        [Column("shopimage")]
        public string ProfileImagePath { get; set; }
        [Column("confirm")]
        public int Is_confirm { get; set; }
        

        
    }
}
