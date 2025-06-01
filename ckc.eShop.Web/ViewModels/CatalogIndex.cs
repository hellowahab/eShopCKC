using Ckc.EShop.ApplicationCore.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ckc.EShop.Web.ViewModels
{
    public class CatalogIndex
    {
        public IEnumerable<CatalogItem> CatalogItems { get; set; }

        public IEnumerable<SelectListItem> Brands { get; set; }

        public IEnumerable<SelectListItem> Types { get; set;}

        public int? BrandFilterApplied { get; set; }

        public int? TypesFilterApplied { get; set; }

        public PaginationInfo PaginationInfo { get; set; }

    }
}
