using System.ComponentModel.DataAnnotations;

namespace IdentityDemo.ViewModels
{
    public class ShopViewModel
    {
        public int ShopId { get; set; }

        [Required]
        public string ShopName { get; set; }

        public string ShopPhone { get; set; }
        public string ShopEmail { get; set; }
        public string ShopAddress { get; set; }
        public string ShopDescription { get; set; }
        //public string ShopImageName { get; set; }
        public string? ShopOwnerName { get; set; }
        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; } // Property for image upload
    }
}
