using Ckc.EShop.Web.Services;
using Ckc.EShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ckc.EShop.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogSvc;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public CatalogController(ICatalogService catalogSvc, Microsoft.AspNetCore.Hosting.IHostingEnvironment env) 
        {
            _catalogSvc = catalogSvc;
            _env  = env;
        }

        public async Task<IActionResult> Index(int? BrandFilterApplied,
            int? TypeFilterApplied, int? page)
        {
            var itemsPage = 10;
            var catalog = await _catalogSvc.GetCatalogItems(page ?? 0,
                itemsPage, BrandFilterApplied, TypeFilterApplied);

            var vm = new CatalogIndex
            {
                CatalogItems = catalog.Data,
                Brands = await _catalogSvc.GetBrands(),
                Types = await _catalogSvc.GetTypes(),
                BrandFilterApplied = BrandFilterApplied ?? 0,
                TypesFilterApplied = TypeFilterApplied ?? 0,
                PaginationInfo = new PaginationInfo()
                { 
                    ActualPage = page ?? 0,
                    ItemsPerPage = catalog.Data.Count,
                    TotalItems = catalog.Count,
                    TotalPages = int.Parse(Math.Ceiling((decimal)catalog.Count /itemsPage).ToString())
                }             

            };

            vm.PaginationInfo.Next = vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1 ? "is-disabled" : "";

            vm.PaginationInfo.Previous = vm.PaginationInfo.ActualPage == 0 ? "is-disabled" : "";


            return View(vm);
        }

        [HttpGet("{id}")]
        [Route("[Controller]/pic/{id}")]

        public IActionResult GetImage(int id)
        {
            var contentRoot = _env.ContentRootPath + "//Pics";
            var path = Path.Combine(contentRoot, id + ".png");
            byte[] b = System.IO.File.ReadAllBytes(path);
            return File(b, "image/png");
        }
    }
}
