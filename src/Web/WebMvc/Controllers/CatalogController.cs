using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroServicesOnDocker.Web.WebMvc.Models;
using MicroServicesOnDocker.Web.WebMvc.Services;
using MicroServicesOnDocker.Web.WebMvc.ViewModels;

namespace MicroServicesOnDocker.Web.WebMvc.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService) => _catalogService = catalogService;


        public async Task<IActionResult> Index(int? brandFilterApplied, int? typeFilterApplied, int? page)
        {
            //TODO: this should be moved to appSetting.json
            int pageSize = 10;
            Catalog catalog = await _catalogService.GetCatalogItem(currentPage: page ?? 1, pageSize: pageSize, brandFilterApplied,
              typeFilterApplied);
            var catalogIndexViewModel = new CatalogIndexViewModel()
            {
                CatalogItems =  catalog.CatalogItems, 
                Brands = await _catalogService.GetBrands(),
                Types = await _catalogService.GetTypes(),
                BrandFilterApplied = brandFilterApplied ??0,
                TypeFilterApplied = typeFilterApplied ??0,
                PaginationInfo = new PaginationInfo()
                {
                    CurrentPage = page ?? 1,
                    PageSize = catalog.CatalogItems.Count ,
                    TotalCount = catalog.TotalCount,
                    TotalPages = (int)Math.Ceiling(((double)catalog.TotalCount /pageSize))
                }
            };
            catalogIndexViewModel.PaginationInfo.Next = (catalogIndexViewModel.PaginationInfo.CurrentPage == catalogIndexViewModel.PaginationInfo.TotalPages) ? "is-disabled" : "";
            catalogIndexViewModel.PaginationInfo.Previous  = (catalogIndexViewModel.PaginationInfo.CurrentPage == 1)? "is-disabled" : "";
            return View(catalogIndexViewModel);
        }
         
        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}