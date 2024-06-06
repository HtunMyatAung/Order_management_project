using Microsoft.AspNetCore.Identity;

namespace Order_management_project.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string userName { get; set; }
    }
}
