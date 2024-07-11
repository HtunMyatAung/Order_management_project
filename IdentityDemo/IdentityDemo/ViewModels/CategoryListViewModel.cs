using IdentityDemo.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityDemo.ViewModels
{
    public class CategoryListViewModel
    {
        public CategoryModel NewCategory { get; set; }
        [BindNever]
        public List<CategoryModel> List { get; set; }
    }
}
