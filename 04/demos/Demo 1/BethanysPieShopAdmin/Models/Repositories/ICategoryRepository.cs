namespace BethanysPieShopAdmin.Models.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
    }
}
