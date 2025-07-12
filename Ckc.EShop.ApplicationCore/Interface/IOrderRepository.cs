using Ckc.EShop.ApplicationCore.Entities.OrderAggregate;

namespace Ckc.EShop.ApplicationCore.Interface
{
    public interface IOrderRepository: IRepository<Order>, IAsyncRepository<Order>
    {
        Order GetByIdWithItems(int id);

        Task<Order> GetByIdWithItemsAsync(int id);
    }
}