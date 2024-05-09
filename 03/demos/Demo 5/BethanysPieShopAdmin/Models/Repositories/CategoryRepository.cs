using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;

        public CategoryRepository(BethanysPieShopDbContext bethanysPieShopDbContext)
        {
            _bethanysPieShopDbContext = bethanysPieShopDbContext;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _bethanysPieShopDbContext.Categories.OrderBy(p => p.CategoryId);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _bethanysPieShopDbContext.Categories.OrderBy(c => c.CategoryId).ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _bethanysPieShopDbContext.Categories.Include(p => p.Pies).FirstOrDefaultAsync(c => c.CategoryId == id);
        }
    }
}
