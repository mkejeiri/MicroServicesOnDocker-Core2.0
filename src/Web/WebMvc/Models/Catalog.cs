using System.Collections.Generic;

namespace MicroServicesOnDocker.Web.WebMvc.Models
{
    public class Catalog
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<CatalogItem> CatalogItems { get; set; }
    }
}
