using IdentityDemo.Models;

namespace IdentityDemo.Repositories
{
    public interface ICategoryRepository
    {
        Task<CategoryModel> GetCategoryByNameAsync(string category);
        Task<List<CategoryModel>> GetAllCategories();
        Task AddCategoryAsync(CategoryModel category);
        Task<List<string>> GetCategoryNamesAsync();
        Task UpdateCategoryAsync(CategoryModel category);
        Task DeleteCategoryAsync(int categoryId);


        }
}
