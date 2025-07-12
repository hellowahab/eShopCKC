using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Entities.OrderAggregate;
using Ckc.EShop.ApplicationCore.Interface;
using Ckc.EShop.ApplicationCore.Specifications;
using Ckc.EShop.Web.Interfaces;

namespace Ckc.EShop.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;

        public OrderService(
            IAsyncRepository<Basket> basketRepository, 
            IAsyncRepository<CatalogItem> itemRepository,
            IAsyncRepository<Order> orderRepository,
            IUriComposer uriComposer) 
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
        }

        public async Task CreateOrderAsync(int basketId, Address shippingAddress)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
                var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name,
                    catalogItem.PictureUri);
                var orderItem = new OrderItem(itemOrdered, item.UnitPrice, item.Quantity);
                items.Add(orderItem);
            }
            var order = new Order(basket.BuyerID, shippingAddress, items);
            await _orderRepository.AddAsync(order);

        }
    }
}
