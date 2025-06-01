using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ckc.EShop.Infrastructure.Data
{
    public class EFRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly CatalogDbContext _dbContext;

        public EFRepository(
            CatalogDbContext dbContext
            ) {
            _dbContext = dbContext;
        }

        public T GetById(int id)
        {
            return _dbContext.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public List<T> List()
        {
            return _dbContext.Set<T>().ToList();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }
        

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
