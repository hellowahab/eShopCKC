namespace Ckc.EShop.ApplicationCore.Entities
{
    public class Basket : BaseEntity
    {
        public string BuyerID { get; set; }


        public List<BasketItems> Items { get; set; } = new List<BasketItems>();

        public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1)
        {
            if (!Items.Any(i => i.CatalogItemId == catalogItemId))
            {
                Items.Add(new BasketItems()
                {
                    CatalogItemId = catalogItemId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
                return;
            }
            var existingItem = Items.FirstOrDefault(i =>  i.CatalogItemId == catalogItemId);

            existingItem.Quantity += quantity;
        }
    }
}