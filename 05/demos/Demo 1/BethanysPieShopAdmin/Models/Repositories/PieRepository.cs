using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Models.Repositories
{
    public class PieRepository : IPieRepository
    {
        private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;

        public PieRepository(BethanysPieShopDbContext bethanysPieShopDbContext)
        {
            _bethanysPieShopDbContext = bethanysPieShopDbContext;
        }

        public async Task<IEnumerable<Pie>> GetAllPiesAsync()
        {
            return await _bethanysPieShopDbContext.Pies.OrderBy(c => c.PieId).AsNoTracking().ToListAsync();
        }

        public async Task<Pie?> GetPieByIdAsync(int pieId)
        {
            return await _bethanysPieShopDbContext.Pies.Include(p => p.Ingredients).Include(p => p.Category).AsNoTracking().FirstOrDefaultAsync(p => p.PieId == pieId);
        }

        public async Task<int> AddPieAsync(Pie pie)
        {
            //throw new Exception("Database down");
            _bethanysPieShopDbContext.Pies.Add(pie);//could be done using async too
            return await _bethanysPieShopDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdatePieAsync(Pie pie)
        {

            var pieToUpdate = await _bethanysPieShopDbContext.Pies.FirstOrDefaultAsync(c => c.PieId == pie.PieId);
            if (pieToUpdate != null)
            {
                pieToUpdate.CategoryId = pie.CategoryId;
                pieToUpdate.ShortDescription = pie.ShortDescription;
                pieToUpdate.LongDescription = pie.LongDescription;
                pieToUpdate.Price = pie.Price;
                pieToUpdate.AllergyInformation = pie.AllergyInformation;
                pieToUpdate.ImageThumbnailUrl = pie.ImageThumbnailUrl;
                pieToUpdate.ImageUrl = pie.ImageUrl;
                pieToUpdate.InStock = pie.InStock;
                pieToUpdate.IsPieOfTheWeek = pie.IsPieOfTheWeek;
                pieToUpdate.Name = pie.Name;

                _bethanysPieShopDbContext.Pies.Update(pieToUpdate);
                return await _bethanysPieShopDbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"The pie to update can't be found.");
            }
        }
    }
}
