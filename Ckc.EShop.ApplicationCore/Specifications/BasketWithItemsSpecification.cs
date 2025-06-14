using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Interface;
using System.Linq.Expressions;

namespace Ckc.EShop.ApplicationCore.Specifications
{
    public class BasketWithItemsSpecification : ISpecification<Basket>
    {
        public BasketWithItemsSpecification(int basketId) 
        {
            BasketId = basketId;
            AddIncludes(b => b.Items);
        }
        public int BasketId { get; }

        public Expression<Func<Basket, bool>> Criteria => b => b.Id == BasketId;
        public List<Expression<Func<Basket, object>>> Includes { get; } = new List<Expression<Func<Basket, object>>>();
        

        public void AddIncludes(Expression<Func<Basket, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
