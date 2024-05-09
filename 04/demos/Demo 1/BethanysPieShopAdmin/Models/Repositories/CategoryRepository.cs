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
            return _bethanysPieShopDbContext.Categories.AsNoTracking().OrderBy(p => p.CategoryId);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _bethanysPieShopDbContext.Categories.AsNoTracking().OrderBy(c => c.CategoryId).ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _bethanysPieShopDbContext.Categories.AsNoTracking().Include(p => p.Pies).FirstOrDefaultAsync(c => c.CategoryId == id);
        }
    }
}
