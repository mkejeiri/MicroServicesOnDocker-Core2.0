namespace MicroServicesOnDocker.Web.WebMvc.Infrastructure
{
    public class ApiPaths
    {
        public static class Cart
        {
            public static string GetCart(string baseUri, string cartId)
            {
                return $"{baseUri}/{cartId}";
            }

            public static string UpdateCart(string baseUri)
            {
                return baseUri;
            }

            public static string CleanCart(string baseUri, string cartId)
            {
                return $"{baseUri}/{cartId}";
            }
        }
        public static class Catalog
        {
            public static string GetAllCatalogItems(string baseUri, int currentPage, int pageSize, int? brand, int? type)
            {
                if (brand.HasValue || type.HasValue)
                {
                    var brandQs = (brand.HasValue) ? brand.Value.ToString() : "null";
                    var typeQs = (type.HasValue) ? type.Value.ToString() : "null";
                    return $"{baseUri}/ItemsTypeBrand/type/{typeQs}/brand/{brandQs}?pageindex={currentPage}&pageSize={pageSize}";
                }
                return $"{baseUri}/getItemsPerPage?pageindex={currentPage}&pageSize={pageSize}";

            }
            public static string GetCatalogItem(string baseUri, int id)
            {
                return $"{baseUri}/items/{id}";
            }

            public static string GetAllBrands(string baseUri)
            {
                return $"{baseUri}/Brands";
            }

            public static string GetAllTypes(string baseUri)
            {
                return $"{baseUri}/Types";
            }
        }
    }
}