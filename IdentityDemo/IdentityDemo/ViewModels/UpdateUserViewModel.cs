
 using System.ComponentModel.DataAnnotations;
namespace IdentityDemo.ViewModels
{
    public class UpdateUserViewModel
    {
        public string? Id { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "First Name")]
        public string? UserName { get; set; }

        [Display(Name = "phone")]
        public string? UserPhone { get; set; }
        public string? Useraddress { get; set; }
        public string? Role { get; set; }
        //[Required]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        public IFormFile? UserImagePath { get; set; }
       // [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        // Add other properties as needed
    }
}

