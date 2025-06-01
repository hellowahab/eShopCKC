using Ckc.EShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ckc.EShop.Web.Services
{
    public interface ICatalogService
    {
        Task<Catalog> GetCatalogItems(int pageIndex, int itemsPage,
            int? brandId, int? typeId);

        Task<IEnumerable<SelectListItem>> GetBrands();

        Task<IEnumerable<SelectListItem>> GetTypes();

    }
}
