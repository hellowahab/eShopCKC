namespace Ckc.EShop.Web.ViewModels
{
    public class BasketViewModel
    {
        public int Id { get; set; }

        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();

        public string BuyerId { get; set; }

        public decimal Total()
        {
            return Math.Round(Items.Sum(i => i.UnitPrice * i.Quantity), 2);
        }
    }
}