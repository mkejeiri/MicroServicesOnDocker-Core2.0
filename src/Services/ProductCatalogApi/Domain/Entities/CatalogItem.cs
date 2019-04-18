namespace ProductCatalogApi.Domain.Entities
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //[Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string PictureFileName { get; set; }
        public string PictureUri { get; set; }

        public int CatalogBrandId { get; set; }
        public int CatalogTypeId { get; set; }

        public CatalogBrand CatalogBrand { get; set; }
        public CatalogType CatalogType { get; set; }
    }
}