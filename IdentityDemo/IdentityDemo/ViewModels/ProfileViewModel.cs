using IdentityDemo.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IdentityDemo.ViewModels
{
    public class ProfileViewModel
    {
        
        public string? UserId { get; set; }
        public string? UserName { get; set; }        
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set;}
        public string? Address { get; set; }
        public int? OrderCount { get; set; }
        //public string? UserImageName { get; set; }
        public IFormFile? UserImage { get; set; } // Property for image upload
        public List<OrderDetailModel>? OrderDetails { get; set; }
        public List<OrderViewModel>? Orders {  get; set; }     
       
        //public string? Order { get; set; }
    }
}
