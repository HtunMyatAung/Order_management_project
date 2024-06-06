
 using System.ComponentModel.DataAnnotations;
namespace IdentityDemo.ViewModels
{
    public class UpdateUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string UserName { get; set; }

        [Display(Name = "phone")]
        public string UserPhone { get; set; }
        public string Useraddress { get; set; }
        public string Password { get; set; }

        // Add other properties as needed
    }
}

