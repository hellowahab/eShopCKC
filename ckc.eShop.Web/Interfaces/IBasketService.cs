using Ckc.EShop.Web.ViewModels;
using Microsoft.Identity.Client;

namespace Ckc.EShop.Web.Interfaces
{
    public interface IBasketService
    {
        //Task<BasketViewModel> GetBasket(int basketId);

        //Task<BasketViewModel> CreateBasket();

        Task<BasketViewModel> GetOrCreateBasketForUser(string userName);

        Task AddItemToBasket(int basketId, int catalogItemId,
            decimal price, int quantity);

        Task Checkout(int basketId);

        Task TransferBasket(string anonymousId, string userName);
    }
}
