using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalogApi.Domain.Entities;

namespace ProductCatalogApi.Data.Extensions
{
    public static class Extensions
    {

        public static void Configure(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatalogBrand>(ConfigureCatalogBrand);
            modelBuilder.Entity<CatalogItem>(ConfigureCatalogItem);
            modelBuilder.Entity<CatalogType>(ConfigureCatalogType);
        }

        private static void ConfigureCatalogType(EntityTypeBuilder<CatalogType> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("CatalogTypes");
            entityTypeBuilder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("Catalog_type_hilo")
                .HasMaxLength(100)
                .IsRequired();

            entityTypeBuilder.Property(c => c.Type).IsRequired();
        }

        private static void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("CatalogItems");
            entityTypeBuilder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("Catalog_hilo")
                .IsRequired();
            entityTypeBuilder.Property(c => c.Name).HasMaxLength(50).IsRequired();
            entityTypeBuilder.Property(c => c.Description).IsRequired();
            entityTypeBuilder.Property(c => c.Price).IsRequired().HasColumnType("decimal(18,2)"); ;
            entityTypeBuilder.Property(c => c.PictureUri).IsRequired(false);
            entityTypeBuilder.Property(c => c.PictureFileName).IsRequired(false);
            entityTypeBuilder.HasOne<CatalogBrand>(c => c.CatalogBrand)
                .WithMany()
                .HasForeignKey(x => x.CatalogBrandId);

            entityTypeBuilder.HasOne(c => c.CatalogType)
                .WithMany()
                .HasForeignKey(x => x.CatalogTypeId);

        }

        private static void ConfigureCatalogBrand(EntityTypeBuilder<CatalogBrand> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("CatalogBrands");
            entityTypeBuilder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("Catalog_brand_hilo")
                .IsRequired();
            entityTypeBuilder.Property(c => c.Brand)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
