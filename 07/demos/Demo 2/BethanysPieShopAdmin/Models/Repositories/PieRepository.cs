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

        public async Task<int> GetAllPiesCountAsync()
        {
            IQueryable<Pie> allPies = from p in _bethanysPieShopDbContext.Pies
                                      select p;
            var count = await allPies.CountAsync();
            return count;
        }

        public async Task<IEnumerable<Pie>> GetPiesPagedAsync(int? pageNumber, int pageSize)
        {
            IQueryable<Pie> pies = from p in _bethanysPieShopDbContext.Pies
                                   select p;

            pageNumber ??= 1;

            pies = pies.Skip((pageNumber.Value - 1) * pageSize).Take(pageSize);

            return await pies.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Pie>> GetPiesSortedAndPagedAsync(string sortBy, int? pageNumber, int pageSize)
        {
            IQueryable<Pie> allPies = from p in _bethanysPieShopDbContext.Pies
                                      select p;
            IQueryable<Pie> pies;

            switch (sortBy)
            {
                case "name_desc":
                    pies = allPies.OrderByDescending(p => p.Name);
                    break;
                case "name":
                    pies = allPies.OrderBy(p => p.Name);
                    break;
                case "id_desc":
                    pies = allPies.OrderByDescending(p => p.PieId);
                    break;
                case "id":
                    pies = allPies.OrderBy(p => p.PieId);
                    break;
                case "price_desc":
                    pies = allPies.OrderByDescending(p => p.Price);
                    break;
                case "price":
                    pies = allPies.OrderBy(p => p.Price);
                    break;
                default:
                    pies = allPies.OrderBy(p => p.PieId);
                    break;
            }

            pageNumber ??= 1;

            pies = pies.Skip((pageNumber.Value - 1) * pageSize).Take(pageSize);

            return await pies.AsNoTracking().ToListAsync(); ;
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

        public async Task<int> DeletePieAsync(int id)
        {
            var pieToDelete = await _bethanysPieShopDbContext.Pies.FirstOrDefaultAsync(c => c.PieId == id);

            if (pieToDelete != null)
            {
                _bethanysPieShopDbContext.Pies.Remove(pieToDelete);
                return await _bethanysPieShopDbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"The pie to delete can't be found.");
            }
        }

        public async Task<IEnumerable<Pie>> SearchPies(string searchQuery, int? categoryId)
        {
            var pies = from p in _bethanysPieShopDbContext.Pies
                       select p;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                pies = pies.Where(s => s.Name.Contains(searchQuery) || s.ShortDescription.Contains(searchQuery) || s.LongDescription.Contains(searchQuery));
            }

            if (categoryId != null)
            {
                pies = pies.Where(s => s.CategoryId == categoryId);
            }

            return await pies.ToListAsync();
        }

    }
}
