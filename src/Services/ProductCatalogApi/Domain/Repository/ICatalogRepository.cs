using System.Collections.Generic;
using System.Threading.Tasks;
using ProductCatalogApi.Domain.Entities;
using ProductCatalogApi.Helpers;

namespace ProductCatalogApi.Domain.Repository
{
    public interface ICatalogRepository
    {
        Task<IReadOnlyList<CatalogType>> GetCatalogTypesAsync();
        Task<IReadOnlyList<CatalogBrand>> GetCatalogBrandsAsync();
        Task<IReadOnlyList<CatalogItem>> GetCatalogItemsAsync();
        Task<CatalogItem> GetItemAsync(int id);
        Task<PagedList<CatalogItem>> GetPagedCatalogItemsAsync(int currentPage, int pageSize);
        Task<PagedList<CatalogItem>> GetPagedCatalogItemsByNameAsync(string name, int currentPage, int pageSize);
        Task<PagedList<CatalogItem>> GetPagedCatalogItemsByTypeBrandAsync(int? catalogTypeId, int? catalogBrandId, int currentPage, int pageSize);
        Task<int> AddAsync(CatalogItem catalogItem);
        Task<bool>Exists(int id);
        Task UpdateCatalogItem(CatalogItem productToUpdate);
        Task DeleteCatalogItem(int id);
    }
}