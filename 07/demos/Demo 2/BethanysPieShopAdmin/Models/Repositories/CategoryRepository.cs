using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BethanysPieShopAdmin.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;
        private IMemoryCache _memoryCache;
        private const string AllCategoriesCacheName = "AllCategories";

        public CategoryRepository(BethanysPieShopDbContext bethanysPieShopDbContext, IMemoryCache memoryCache)
        {
            _bethanysPieShopDbContext = bethanysPieShopDbContext;
            _memoryCache = memoryCache;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _bethanysPieShopDbContext.Categories.AsNoTracking().OrderBy(p => p.CategoryId);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            List<Category> allCategories = null;

            if (!_memoryCache.TryGetValue(AllCategoriesCacheName, out allCategories))
            {
                allCategories = await _bethanysPieShopDbContext.Categories.AsNoTracking().OrderBy(c => c.CategoryId).ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(60));

                _memoryCache.Set(AllCategoriesCacheName, allCategories, cacheEntryOptions);
            }
          
            return allCategories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _bethanysPieShopDbContext.Categories.AsNoTracking().Include(p => p.Pies).FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            bool categoryWithSameNameExist = await _bethanysPieShopDbContext.Categories.AnyAsync(c => c.Name == category.Name);

            if (categoryWithSameNameExist)
            {
                throw new Exception("A category with the same name already exists");
            }

            _bethanysPieShopDbContext.Categories.Add(category);//could be done using async too

            return await _bethanysPieShopDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateCategoryAsync(Category category)
        {
            bool categoryWithSameNameExist = await _bethanysPieShopDbContext.Categories.AnyAsync(c => c.Name == category.Name && c.CategoryId != category.CategoryId);

            if (categoryWithSameNameExist)
            {
                throw new Exception("A category with the same name already exists");
            }

            var categoryToUpdate = await _bethanysPieShopDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);

            if (categoryToUpdate != null)
            {

                categoryToUpdate.Name = category.Name;
                categoryToUpdate.Description = category.Description;

                _bethanysPieShopDbContext.Categories.Update(categoryToUpdate);
                return await _bethanysPieShopDbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"The category to update can't be found.");
            }
        }

        public async Task<int> DeleteCategoryAsync(int id)
        {
            //throw new Exception("Database down");

            var categoryToDelete = await _bethanysPieShopDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

            if (categoryToDelete != null)
            {
                _bethanysPieShopDbContext.Categories.Remove(categoryToDelete);
                return await _bethanysPieShopDbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"The category to delete can't be found.");
            }
        }

        public async Task<int> UpdateCategoryNamesAsync(List<Category> categories)
        {
            foreach (var category in categories)
            {
                var categoryToUpdate = await _bethanysPieShopDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);

                if (categoryToUpdate != null)
                {
                    categoryToUpdate.Name = category.Name;

                    _bethanysPieShopDbContext.Categories.Update(categoryToUpdate);
                }
            }

            return await _bethanysPieShopDbContext.SaveChangesAsync();
        }
    }
}
