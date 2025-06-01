using Ckc.EShop.ApplicationCore.Entities;
using Ckc.EShop.ApplicationCore.Interface;
using Ckc.EShop.Infrastructure.Data;
using Ckc.EShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ckc.EShop.Web.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IRepository<CatalogBrand> _brandRepository;
        private readonly IRepository<CatalogType> _typeRepository;

        public CatalogService(
            IRepository<CatalogItem> itemRepository,
            IRepository<CatalogBrand> brandRepository,
            IRepository<CatalogType> typeRepository
            ) {
            _itemRepository = itemRepository;
            _brandRepository = brandRepository;
            _typeRepository = typeRepository;
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var brands = _brandRepository.List();
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem()
            {
                Value = null,
                Text = "All",
                Selected = true,
            });
            foreach (var brand in brands)
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Id.ToString(),
                    Text = brand.Brand
                });
            }
            return items;
        }

        public async Task<Catalog> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
        {
            var root = _itemRepository.List();

            //if (typeId.HasValue)
            //{
            //    root = root.Where(ci => ci.CatalogTypeId == typeId.Value);
            //}

            //if (brandId.HasValue)
            //{
            //    root = root.Where(ci => ci.CatalogBrandId == brandId.Value);
            //}

            var totalItems = root.Count;

            var itemOnPage = root
                .Skip(itemsPage * pageIndex)
                .Take(itemsPage)
                .ToList();

            itemOnPage = ComposePictureUri(itemOnPage);

            return new Catalog
            {
                Data = itemOnPage,
                PageIndex = pageIndex,
                Count = (int)totalItems
            };

        }

        private List<CatalogItem> ComposePictureUri(List<CatalogItem> items)
        {
            var baseUri = "https://localhost:7164";
            items.ForEach(x =>
                     x.PictureUri = x.PictureUri.Replace("http://catalogbaseurltobereplaced", baseUri)

                );
               
            return items;

        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var types = _typeRepository.List();
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem()
            {
                Value = null,
                Text = "All",
                Selected = true,
            });
            foreach (var type in types)
            {
                items.Add(new SelectListItem()
                {
                    Value = type.Id.ToString(),
                    Text = type.Type
                });
            }
            return items;
        }
    }
}
