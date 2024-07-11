using IdentityDemo.Data;
using IdentityDemo.Models;

namespace IdentityDemo.Repositories
{
    public class CategoryRepository:ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<CategoryModel>> GetAllCategories()
        {
            return _context.Categories.ToList();
        }
        public async Task AddCategoryAsync(CategoryModel category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

    }
}
