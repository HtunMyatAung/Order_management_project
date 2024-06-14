using System.ComponentModel.DataAnnotations;

namespace IdentityDemo.ViewModels
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
