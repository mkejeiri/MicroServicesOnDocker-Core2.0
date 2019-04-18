namespace MicroServicesOnDocker.Web.WebMvc.Infrastructure
{
    public class ApiPaths
    {
        public static class Catalog
        {
            public static string GetAllCatalogItems(string baseUri, int page, int take, int? brand, int? type)
            {
                var filterQs = "";
                if (brand.HasValue || type.HasValue)
                {
                    var brandQs = (brand.HasValue) ? brand.Value.ToString() : "null";
                    var typeQs = (type.HasValue) ? type.Value.ToString() : "null";

                    filterQs = $"/type/{typeQs}/brand/{brandQs}";
                }

                return $"{baseUri}/items{filterQs}?pageindex={page}&pagesize={take}";
            }

            public static string GetCatalogItem(string baseUri, int id)
            {
                return $"{baseUri}/items/{id}";
            }

            public static string GetAllBrands(string baseUri)
            {
                return $"{baseUri}/CatalogBrands";
            }

            public static string GetAllTypes(string baseUri)
            {
                return $"{baseUri}/CatalogTypes";
            }
        }
    }
}