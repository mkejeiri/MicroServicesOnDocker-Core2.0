using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MicroServicesOnDocker.Web.WebMvc.Models;

namespace MicroServicesOnDocker.Services.WebMvc.ViewModels
{
    public class CatalogIndexViewModel
    {
        public IEnumerable<CatalogItem> CatalogItems { get; set; }
        public IEnumerable<SelectListItem> Brands { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public int? BrandFilterApplied { get; set; }
        public int? TypeFilterApplied { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
