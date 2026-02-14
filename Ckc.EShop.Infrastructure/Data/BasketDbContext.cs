using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ckc.EShop.Infrastructure.Data
{
    public class BasketDbContext: DbContext
    {
        public BasketDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Basket> Baskets { get; set; }
    }
}
