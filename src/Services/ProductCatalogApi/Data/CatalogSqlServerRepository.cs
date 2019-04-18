using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Domain.Entities;
using ProductCatalogApi.Domain.Repository;
using ProductCatalogApi.Helpers;

namespace ProductCatalogApi.Data
{
    public class CatalogSqlServerRepository : ICatalogRepository
    {
        private readonly CatalogDbContext _context;

        public CatalogSqlServerRepository(CatalogDbContext context)
        {
            _context = context;
            //This a readonly catalog db
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<IReadOnlyList<CatalogType>> GetCatalogTypesAsync() =>
            await _context.CatalogTypes.ToListAsync();

        public async Task<IReadOnlyList<CatalogBrand>> GetCatalogBrandsAsync() =>
            await _context.CatalogBrands.ToListAsync();

        public async Task<IReadOnlyList<CatalogItem>> GetCatalogItemsAsync() =>
            await _context.CatalogItems.ToListAsync();

        public async Task<CatalogItem> GetItemAsync(int id) =>
            await _context.CatalogItems.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<PagedList<CatalogItem>> GetPagedCatalogItemsAsync(int currentPage, int pageSize)
            => await PagedList<CatalogItem>.CreateAsync(_context.CatalogItems, currentPage, pageSize);

        //TODO: Make a generic one that could take a list of predicates!
        public async Task<PagedList<CatalogItem>> GetPagedCatalogItemsByNameAsync(string name, int currentPage,
            int pageSize)
            => await PagedList<CatalogItem>.CreateAsync(_context.CatalogItems.Where(x => x.Name.StartsWith(name)),
                currentPage, pageSize);

        public async Task<PagedList<CatalogItem>> GetPagedCatalogItemsByTypeBrandAsync(
            int? catalogTypeId, int? catalogBrandId, int currentPage, int pageSize) =>
                 await PagedList<CatalogItem>.CreateAsync(_context.CatalogItems
                .Where(x => (!catalogTypeId.HasValue || x.CatalogTypeId == catalogTypeId.Value) &&
                            (!catalogBrandId.HasValue || x.CatalogBrandId == catalogBrandId.Value)),
        currentPage, pageSize);

        public async Task<int> AddAsync(CatalogItem catalogItem)
        {
            _context.CatalogItems.Add(catalogItem);
            await _context.SaveChangesAsync();
            return catalogItem.Id;
        }

        public async Task<bool> Exists(int id) =>
            await _context.CatalogItems.AnyAsync(x => x.Id == id);

        public async Task UpdateCatalogItem(CatalogItem productToUpdate)
        {
            _context.CatalogItems.Update(productToUpdate);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCatalogItem(int id)
        {
            _context.CatalogItems.Remove(await _context.CatalogItems.FirstOrDefaultAsync(x => x.Id == id));
            await _context.SaveChangesAsync();
        }
    }
}