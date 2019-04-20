using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MicroServicesOnDocker.Web.WebMvc.Models;

namespace MicroServicesOnDocker.Services.WebMvc.Services
{
    public interface ICatalogService
    {
        Task<Catalog> GetCatalogItem(int currentPage, int pageSize, int? brand, int? type);
        Task<IEnumerable<SelectListItem>> GetBrands();
        Task<IEnumerable<SelectListItem>> GetTypes();
    }
}