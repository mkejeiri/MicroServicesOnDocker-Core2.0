using System.Collections.Generic;
using System.Threading.Tasks;
using MicroServicesOnDocker.Services.ProductCatalogApi.Domain.Entities;
using MicroServicesOnDocker.Services.ProductCatalogApi.Helpers;

namespace MicroServicesOnDocker.Services.ProductCatalogApi.Domain.Repository
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