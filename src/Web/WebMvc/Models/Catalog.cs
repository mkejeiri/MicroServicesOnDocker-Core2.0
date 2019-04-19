﻿using System.Collections.Generic;

namespace MicroServicesOnDocker.Web.WebMvc.Models
{
    public class Catalog
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<CatalogItem> CatalogItems { get; set; }
    }
}
