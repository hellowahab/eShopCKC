using Ckc.EShop.ApplicationCore.Entities.OrderAggregate;
using Ckc.EShop.ApplicationCore.Interface;
using System.Linq.Expressions;

namespace Ckc.EShop.ApplicationCore.Specifications
{
    public class CustomerOrderWithItemsSpecification :ISpecification<Order>
    {
        private readonly string _buyerId;

        public CustomerOrderWithItemsSpecification(string buyerId)
        {
            _buyerId = buyerId;
            AddIncludes(o => o.OrderItems);
            AddIncludes("OrderItems.ItemOrdered");
            
        }

        public Expression<Func<Order, bool>> Criteria => o => o.BuyerId == _buyerId;
        public List<Expression<Func<Order, object>>> Includes { get; } = new List<Expression<Func<Order, object>>>();

        public List<string> IncludeStrings { get; } = new List<string>();

        public void AddIncludes(Expression<Func<Order, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public void AddIncludes(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

    }
}