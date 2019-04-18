using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Data.Extensions;
using ProductCatalogApi.Domain.Entities;

namespace ProductCatalogApi.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions options) : base(options)
        {
            //put here the db config through the fluent api
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configure();
        }

        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }
    }
}
 