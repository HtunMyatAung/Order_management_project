using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityDemo.Models
{
    [Table("category")]
    public class CategoryModel
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Column("item_count")]
        public int Item_count { get; set; }

    }
}
