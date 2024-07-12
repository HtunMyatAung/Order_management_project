using IdentityDemo.Data;
using IdentityDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Repositories
{
    public class CategoryRepository:ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CategoryModel> GetCategoryByNameAsync(string category)
        {
            return  await _context.Categories.FirstOrDefaultAsync(c => c.Name == category);
        }
        public async Task<List<string>> GetCategoryNamesAsync()
        {
            return await _context.Categories.Select(c => c.Name).ToListAsync();
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
        public async Task UpdateCategoryAsync(CategoryModel category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
