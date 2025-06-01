using Ckc.EShop.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ckc.EShop.ApplicationCore.Interface
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(int id);

        List<T> List();

        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
