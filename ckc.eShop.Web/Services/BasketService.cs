using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Interface;
using Ckc.EShop.ApplicationCore.Specifications;
using Ckc.EShop.Web.Interfaces;
using Ckc.EShop.Web.ViewModels;

namespace Ckc.EShop.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly IRepository<Basket> _basketRepository;
        private readonly IRepository<CatalogItem> _itemRepository;

        public BasketService(IRepository<Basket> basketRepository,
            IRepository<CatalogItem> itemRepository) 
        {
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
        }

        public async Task<BasketViewModel> GetBasket(int basketId)
        {
            var basketSpec = new BasketWithItemsSpecification(basketId);
            var basket = _basketRepository.List(basketSpec).FirstOrDefault();
            if (basket == null)
            {
                return await CreateBasket();
            }

            var viewModel = new BasketViewModel();
            viewModel.Id = basket.Id;    
            viewModel.BuyerId = basket.BuyerID;
            viewModel.Items = basket.Items.Select(i =>
                            {
                                var itemModel = new BasketItemViewModel()
                                {
                                    Id = i.Id,
                                    UnitPrice = i.UnitPrice,
                                    Quantity = i.Quantity,
                                    CatalogItemId = i.CatalogItemId
                                };

                                var item = _itemRepository.GetById(i.CatalogItemId);
                                itemModel.PictureUrl = item.PictureUri;
                                itemModel.ProductName = item.Name;
                                return itemModel;                 

                            }).ToList();

            return viewModel;
           
        }

        public Task<BasketViewModel> CreateBasket()
        {
            return CreateBasketForUser(null);
        }

        public async Task<BasketViewModel> CreateBasketForUser(string userId)
        {
           var basket = new Basket() { BuyerID = userId };
            _basketRepository.Add(basket);

            return new BasketViewModel()
            {
                BuyerId = basket.BuyerID,
                Id = basket.Id,
                Items = new List<BasketItemViewModel>()
            };
        }


        public async Task AddItemToCart(int basketId, int catalogItemId, decimal price, int quantity)
        {
            var basket = _basketRepository.GetById(basketId); 
            basket.AddItem(catalogItemId, price, quantity);
            _basketRepository.Update(basket);
        }

        public async Task Checkout(int basketId)
        {
            var basket = _basketRepository.GetById(basketId);
            _basketRepository.Delete(basket);
        }

        

        

        
    }
}
