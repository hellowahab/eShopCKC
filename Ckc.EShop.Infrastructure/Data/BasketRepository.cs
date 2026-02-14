using Ckc.EShop.ApplicationCore.Entities.OrderAggregate;
using Ckc.EShop.ApplicationCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ckc.EShop.Infrastructure.Data
{
    public class BasketRepository : BaseRepository<Order>
    {
        private readonly BasketDbContext _dbContext;
        public BasketRepository(BasketDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
