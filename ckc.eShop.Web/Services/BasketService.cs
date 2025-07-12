using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Interface;
using Ckc.EShop.ApplicationCore.Specifications;
using Ckc.EShop.Web.Interfaces;
using Ckc.EShop.Web.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace Ckc.EShop.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IUriComposer _uriComposer;

        public BasketService(IAsyncRepository<Basket> basketRepository,
            IRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer) 
        {
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
            _uriComposer = uriComposer;
        }

        //public async Task<BasketViewModel> GetBasket(int basketId)
        //{
        //    var basketSpec = new BasketWithItemsSpecification(basketId);
        //    var basket = _basketRepository.List(basketSpec).FirstOrDefault();
        //    if (basket == null)
        //    {
        //        return await CreateBasket();
        //    }

        //    var viewModel = new BasketViewModel();
        //    viewModel.Id = basket.Id;    
        //    viewModel.BuyerId = basket.BuyerID;
        //    viewModel.Items = basket.Items.Select(i =>
        //                    {
        //                        var itemModel = new BasketItemViewModel()
        //                        {
        //                            Id = i.Id,
        //                            UnitPrice = i.UnitPrice,
        //                            Quantity = i.Quantity,
        //                            CatalogItemId = i.CatalogItemId
        //                        };

        //                        var item = _itemRepository.GetById(i.CatalogItemId);
        //                        itemModel.PictureUrl = _uriComposer.ComposePicUri(item.PictureUri);
        //                        itemModel.ProductName = item.Name;
        //                        return itemModel;                 

        //                    }).ToList();

        //    return viewModel;
           
        //}

        //public Task<BasketViewModel> CreateBasket()
        //{
        //    return GetOrCreateBasketForUser(null);
        //}

        public async Task<BasketViewModel> GetOrCreateBasketForUser(string userName)
        {
            var basketSpec = new BasketWithItemsSpecification(userName);
            var basket = (await _basketRepository.ListAsync(basketSpec)).FirstOrDefault();

            if (basket == null)
            {
                return await CreateBasketForUser(userName);
            }
            return CreateViewModelFromBasket(basket);
            
        }

        private async Task<BasketViewModel> CreateBasketForUser(string userId)
        {
            var basket = new Basket() { BuyerID = userId };
            await _basketRepository.AddAsync(basket);
            return new BasketViewModel()
            {
                BuyerId = basket.BuyerID,
                Id = basket.Id,
                Items = new List<BasketItemViewModel>()
            };
        }

        private BasketViewModel CreateViewModelFromBasket(Basket basket)
        {
            var viewModel = new BasketViewModel();
            viewModel.Id = basket.Id;
            viewModel.BuyerId = basket.BuyerID;
            viewModel.Items = basket.Items.Select(i =>
            {
                var itemModel = new BasketItemViewModel
                {
                    Id = i.Id,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    CatalogItemId = i.CatalogItemId
                };
                var item = _itemRepository.GetById(i.CatalogItemId);
                itemModel.PictureUrl = _uriComposer.ComposePicUri(item.PictureUri);
                itemModel.ProductName = item.Name;
                return itemModel;
            }).ToList();
            return viewModel;
          
        
        
        
        
        }

        public async Task AddItemToBasket(int basketId, int catalogItemId, decimal price, int quantity)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId); 
            basket.AddItem(catalogItemId, price, quantity);
            _basketRepository.UpdateAsync(basket);
        }

        public async Task Checkout(int basketId)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);
            await _basketRepository.DeleteAsync(basket);
        }

        public async Task TransferBasket(string anonymousId, string userName)
        {
            var basketSpec = new BasketWithItemsSpecification(anonymousId);
            var basket = (await _basketRepository.ListAsync(basketSpec)).FirstOrDefault();
            if (basket == null)
            {
                return;
            }

            basket.BuyerID = userName;
            await _basketRepository.UpdateAsync(basket);

        }

        public Task TransferBasketAsync(string anonymousId, string userName)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteBasketAsync(int basketId)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);
            await _basketRepository.DeleteAsync(basket);
        }

        public Task SetQuantities(int basketID, Dictionary<string, int> quantities)
        {
            throw new NotImplementedException();
        }
    }
}
