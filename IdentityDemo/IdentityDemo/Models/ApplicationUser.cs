using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityDemo.Models
{
    public class ApplicationUser:IdentityUser
    {
       /* public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity =await manager.CreateIdentityAsync(this,DefaultAuthenticationTypes.ApplicationCookie);
        }*/
    }
}
