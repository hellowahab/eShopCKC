using Ckc.EShop.ApplicationCore.Entities.OrderAggregate;
using Ckc.EShop.Infrastructure.Identity;
using Ckc.EShop.Web.Interfaces;
using Ckc.EShop.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ckc.EShop.Web.Controllers
{
    [Route("[Controller]/[action]")]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOrderService _orderService;
        private const string _basketSessionKey = "basketId";

        public BasketController(IBasketService basketService,
            SignInManager<ApplicationUser> signInManager,
            IOrderService orderService) 
        {
            _basketService = basketService;
            _signInManager = signInManager;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var basketModel = await GetBasketViewModelAsync();
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
            var basket = await GetBasketViewModelAsync();
            await _basketService.AddItemToBasket(basket.Id, productDetails.Id,
                productDetails.Price, 1);
            return RedirectToAction("Index");        
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var basketViewModel = await GetBasketViewModelAsync();
            await _orderService.CreateOrderAsync(basketViewModel.Id,
                new Address("street", "city", "state", "country", "1234"));
           
            //await _basketService.Checkout(basketViewModel.Id);
            await _basketService.DeleteBasketAsync(basketViewModel.Id);
            return View("Checkout");
        }


        private async Task<BasketViewModel> GetBasketViewModelAsync()
        {

            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                return await _basketService.GetOrCreateBasketForUser(User.Identity.Name);
            }

            string anonymousId = GetorSetBasketCookie();
            return await _basketService.GetOrCreateBasketForUser(anonymousId);
                      
        }

        private string GetorSetBasketCookie()
        {
            if (Request.Cookies.ContainsKey(Constants.Basket_CookieName))
            {
                return Request.Cookies[Constants.Basket_CookieName];
            }
            string anonymousID = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Today.AddYears(1);
            Response.Cookies.Append(Constants.Basket_CookieName, anonymousID, cookieOptions);
            return anonymousID;
        }
    }
}
