using Ckc.EShop.ApplicationCore.Entities.OrderAggregate;

namespace Ckc.EShop.Web.ViewModels
{
    public class OrderViewModel
    {
        public int OrderNumber { get; set; }

        public DateTimeOffset OrderDate { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; }

        public Address ShippingAddress { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; }

    }
}
