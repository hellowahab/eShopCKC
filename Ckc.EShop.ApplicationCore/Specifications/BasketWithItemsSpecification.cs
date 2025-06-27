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

        public BasketWithItemsSpecification(string buyerID)
        {
            BuyerID = buyerID;
            AddIncludes(b => b.Items);
        }
        public int? BasketId { get; }

        public string BuyerID { get; }

        public Expression<Func<Basket, bool>> Criteria => b =>
        (BasketId.HasValue && b.Id == BasketId.Value)
        || (BuyerID != null && b.BuyerID == BuyerID);

        public List<Expression<Func<Basket, object>>> Includes { get; } = new List<Expression<Func<Basket, object>>>();
        

        public void AddIncludes(Expression<Func<Basket, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
