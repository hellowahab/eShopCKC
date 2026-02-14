using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Interface;
using Microsoft.EntityFrameworkCore;

namespace Ckc.EShop.Infrastructure.Data
{
    public class EFRepository<T> : BaseRepository<T> where T : BaseEntity
    {
        public EFRepository(CatalogDbContext dbContext) : base(dbContext)
        {
            
        }
    }
}
