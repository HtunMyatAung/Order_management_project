using IdentityDemo.Models;

namespace IdentityDemo.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<CategoryModel>> GetAllCategories();
        Task AddCategoryAsync(CategoryModel category);
    }
}
