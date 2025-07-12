using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Interface;
using Microsoft.EntityFrameworkCore;

namespace Ckc.EShop.Infrastructure.Data
{
    public class EFRepository<T> : IRepository<T>, IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly CatalogDbContext _dbContext;

        public EFRepository(
            CatalogDbContext dbContext
            ) {
            _dbContext = dbContext;
        }

        public virtual T GetById(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> ListAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public IEnumerable<T> List(ISpecification<T> spec)
        {
            var queryableResultWithInclude = spec.Includes
                .Aggregate(_dbContext.Set<T>().AsQueryable(),
                (current, include) => current.Include(include));

            return queryableResultWithInclude
                    .Where(spec.Criteria)
                    .ToList();

        }

        public async Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            var queryableResultWithInclude = spec.Includes
                .Aggregate(_dbContext.Set<T>().AsQueryable(),
                (current, include) => current.Include(include));

            return await queryableResultWithInclude
                .Where(spec.Criteria)
                .ToListAsync();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }
              

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
