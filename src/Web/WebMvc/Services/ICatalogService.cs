using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MicroServicesOnDocker.Web.WebMvc.Models;

namespace MicroServicesOnDocker.Web.WebMvc.Services
{
    interface ICatalogService
    {
        Task<Catalog> GetCatalogItem(int page, int take, int? brand, int? type);
        Task<IEnumerable<SelectListItem>> GetBrands();
        Task<IEnumerable<SelectListItem>> GetTypes();
    }
}