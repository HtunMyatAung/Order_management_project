using System.ComponentModel.DataAnnotations;

namespace IdentityDemo.ViewModels
{
    public class RegisterViewModel
    {
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
       
        
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [StringLength(11)]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]

        public string ConfirmPassword { get; set; }
        [Display(Name = "User Photo")]
        public IFormFile? UserPhoto { get; set; }
        public string? UserImageName {  get; set; }
        // Property to indicate whether the user is registering as a shop owner
        [Display(Name = "Register As")]
        public string? RegisterAs { get; set; }

        // Properties for shop owner 
        public int? ShopId { get; set; }
        public string? ShopName { get; set; }
        public string? ShopPhone { get; set; }
        public string? ShopDescription { get; set; }
        public string? ShopEmail { get; set; }
        public string? ShopAddress { get; set; }

    }
}
