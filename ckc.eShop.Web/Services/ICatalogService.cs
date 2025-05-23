using eShopCKC.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eShopCKC.Services
{
    public interface ICatalogService
    {
        Task<Catalog> GetCatalogItems(int pageIndex, int itemsPage,
            int? brandId, int? typeId);

        Task<IEnumerable<SelectListItem>> GetBrands();

        Task<IEnumerable<SelectListItem>> GetTypes();

    }
}
