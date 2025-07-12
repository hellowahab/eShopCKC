using Ckc.EShop.ApplicationCore.Entities.OrderAggregate;
using Ckc.EShop.ApplicationCore.Interface;
using Microsoft.EntityFrameworkCore;

namespace Ckc.EShop.Infrastructure.Data
{
    public class OrderRepository: EFRepository<Order>, IOrderRepository
    {
        public OrderRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }

        public Order GetByIdWithItems(int id)
        {
            return
                _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ItemOrdered)
                .FirstOrDefault(o => o.Id == id);

        }

        public Task<Order> GetByIdWithItemsAsync(int id)
        {
            return
                _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ItemOrdered)
                .FirstOrDefaultAsync(o => o.Id == id);

        }



    }
}
