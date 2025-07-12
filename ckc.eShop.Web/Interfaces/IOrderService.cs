using Ckc.EShop.ApplicationCore.Entities.OrderAggregate;

namespace Ckc.EShop.Web.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(int basketId, Address shippingAddress);
    }
}