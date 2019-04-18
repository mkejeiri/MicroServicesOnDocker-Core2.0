using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Domain.Entities;

namespace ProductCatalogApi.Data
{
    public class CatalogSeeder
    {

        public static async Task SeedAsync(CatalogDbContext context)
        {
            context.Database.Migrate();
            if (!context.CatalogBrands.Any() && !context.CatalogTypes.Any() && !context.CatalogItems.Any())
            {
                context.CatalogBrands.AddRange(GetCatalogBrands());
                await context.SaveChangesAsync();

                context.CatalogTypes.AddRange(GeCatalogTypes());
                await context.SaveChangesAsync();

                context.CatalogItems.AddRange(GetCatalogItems());
                await context.SaveChangesAsync();
            }
        }

        public static IEnumerable<CatalogItem> GetCatalogItems() =>
          new List<CatalogItem>()
            {
                  new CatalogItem() { CatalogTypeId=2,CatalogBrandId=3, Description = "Shoes for next century", Name = "World Star", Price = 199.5M, PictureUri = "[externalBaseUrl]/api/images/1" },
                new CatalogItem() { CatalogTypeId=1,CatalogBrandId=2, Description = "will make you world champions", Name = "White Line", Price= 88.50M, PictureUri = "[externalBaseUrl]/api/images/2" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=3, Description = "You have already won gold medal", Name = "Prism White Shoes", Price = 129, PictureUri = "[externalBaseUrl]/api/images/3" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, Description = "Olympic runner", Name = "Foundation Hitech", Price = 12, PictureUri = "[externalBaseUrl]/api/images/4" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=1, Description = "Roslyn Red Sheet", Name = "Roslyn White", Price = 188.5M, PictureUri = "[externalBaseUrl]/api/images/5" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, Description = "Lala Land", Name = "Blue Star", Price = 112, PictureUri = "[externalBaseUrl]/api/images/6" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=1, Description = "High in the sky", Name = "Roslyn Green", Price = 212, PictureUri = "[externalBaseUrl]/api/images/7"  },
                new CatalogItem() { CatalogTypeId=1,CatalogBrandId=1, Description = "Light as carbon", Name = "Deep Purple", Price = 238.5M, PictureUri = "[externalBaseUrl]/api/images/8" },
                new CatalogItem() { CatalogTypeId=1,CatalogBrandId=2, Description = "High Jumper", Name = "Addidas<White> Running", Price = 129, PictureUri = "[externalBaseUrl]/api/images/9" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=3, Description = "Dunker", Name = "Elequent", Price = 12, PictureUri = "[externalBaseUrl]/api/images/10" },
                new CatalogItem() { CatalogTypeId=1,CatalogBrandId=2, Description = "All round", Name = "Inredeible", Price = 248.5M, PictureUri = "[externalBaseUrl]/api/images/11" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=1, Description = "Pricesless", Name = "London Sky", Price = 412, PictureUri = "[externalBaseUrl]/api/images/12" },
                new CatalogItem() { CatalogTypeId=3,CatalogBrandId=3, Description = "Tennis Star", Name = "Elequent", Price = 123, PictureUri = "[externalBaseUrl]/api/images/13" },
                new CatalogItem() { CatalogTypeId=3,CatalogBrandId=2, Description = "Wimbeldon", Name = "London Star", Price = 218.5M, PictureUri = "[externalBaseUrl]/api/images/14" },
                new CatalogItem() { CatalogTypeId=3,CatalogBrandId=1, Description = "Rolan Garros", Name = "Paris Blues", Price = 312, PictureUri = "[externalBaseUrl]/api/images/15" }
    };

        public static IEnumerable<CatalogBrand> GetCatalogBrands() =>
        new List<CatalogBrand>()
        {
            new CatalogBrand() {Brand = "Addidas"},
            new CatalogBrand() {Brand = "Nike"},
            new CatalogBrand() {Brand = "Puma"},
            new CatalogBrand() {Brand = "Slazenger"}
        };

        public static IEnumerable<CatalogType> GeCatalogTypes() =>
            new List<CatalogType>()
            {
                new CatalogType() {Type = "Running"},
                new CatalogType() {Type = "Basketball"},
                new CatalogType() {Type = "Tennis"}
            };

    }
}
