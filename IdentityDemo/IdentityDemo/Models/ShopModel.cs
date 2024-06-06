using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityDemo.Models
{
    [Table("shop")]
    public class ShopModel
    {
        [Key]
        [Column("id")]
        //[Required]
        public int ShopId { get; set; }
        [Column("name")]
        //[Required]
        public string ShopName { get; set; }
        [Column("description")]
        //[Required]
        public string ShopDescription { get; set; }
        [Column("phone")]
        //[Required]
        public string ShopPhone{ get; set; }
        [Column("email")]
       // [Required]
        public string ShopEmail{ get; set;}
        [Column("address")]
        //[Required]
        public string ShopAddress { get; set; }

    }
}
