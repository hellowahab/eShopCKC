using Ckc.EShop.Web.Interfaces;
using Ckc.EShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ckc.EShop.Web.Controllers
{
    [Route("[Controller]/[action]")]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private const string _basketSessionKey = "basketId";

        public BasketController(IBasketService basketService) 
        {
            _basketService = basketService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var basketModel = await GetBasketFromSessionAsync();
            return View(basketModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket
            (CatalogItemViewModel productDetails)
        {
            if (productDetails?.Id == null)
            {
                return RedirectToAction("Index", "Catalog");
            }
            var basket = await GetBasketFromSessionAsync();
            await _basketService.AddItemToBasket(basket.Id, productDetails.Id,
                productDetails.Price, 1);
            return RedirectToAction("Index");        
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var basket = await GetBasketFromSessionAsync();
            await _basketService.Checkout(basket.Id);
            return View("Checkout");
        }


        private async Task<BasketViewModel> GetBasketFromSessionAsync()
        {
            string basketId = HttpContext.Session.GetString(_basketSessionKey);
            BasketViewModel basket = null;
            if (basketId == null)
            {
                basket = await _basketService.CreateBasketForUser(User.Identity.Name);
                HttpContext.Session.SetString(_basketSessionKey, basket.Id.ToString());
            }
            else
            { 
                basket = await _basketService.GetBasket(int.Parse(basketId));

            }
            return basket;
        }
    }
}
