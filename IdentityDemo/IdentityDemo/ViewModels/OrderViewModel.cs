using IdentityDemo.Models;

namespace IdentityDemo.ViewModels
{
    public class OrderViewModel
    {
        public int? OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal? OrderPrice { get; set; }
        public int? Shop_Id { get; set; }
        public string? User_Name { get; set; }
        public string? Shop_Name { get; set; }
    }
}
