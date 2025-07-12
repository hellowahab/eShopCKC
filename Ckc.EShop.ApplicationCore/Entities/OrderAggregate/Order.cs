using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Interface;

namespace Ckc.EShop.ApplicationCore.Entities.OrderAggregate
{
    // Root Object
    public class Order: BaseEntity, IAggregateRoot 
    {
        public string BuyerId { get; private set; }
        public Address ShipToAddress { get; private set; }

        private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.Now;

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        private Order()
        { 
        }

        public Order(string buyerId, Address shipToAddress, List<OrderItem> items)
        {
            ShipToAddress = shipToAddress;
            _orderItems = items;
            BuyerId = buyerId;
        }

        public decimal Total()
        {
            var total = 0m;
            foreach (var item in _orderItems)
            {
                total += item.UnitPrice * item.Units;
            }
            return total;
        }


        

        

        
    }
}