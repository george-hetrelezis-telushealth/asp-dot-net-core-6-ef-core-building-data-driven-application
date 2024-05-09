using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;

        public OrderRepository(BethanysPieShopDbContext bethanysPieShopDbContext)
        {
            _bethanysPieShopDbContext = bethanysPieShopDbContext;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _bethanysPieShopDbContext.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Pie).OrderBy(o => o.OrderId).ToListAsync();
        }

        public async Task<Order?> GetOrderDetailsAsync(int? orderId)
        {
            if (orderId != null)
            {
                var order = await _bethanysPieShopDbContext.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Pie).OrderBy(o => o.OrderId).Where(o => o.OrderId == orderId.Value).FirstOrDefaultAsync();

                return order;
            }
            return null;
        }
    }
}
