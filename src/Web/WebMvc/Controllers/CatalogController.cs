﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MicroServicesOnDocker.Services.WebMvc.ViewModels;
using MicroServicesOnDocker.Web.WebMvc.Models;
using MicroServicesOnDocker.Web.WebMvc.Services;

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
            Catalog catalog = await _catalogService.GetCatalogItem(page: page ?? 1, take: pageSize, brandFilterApplied,
              typeFilterApplied);
            var catalogIndexViewModel = new CatalogIndexViewModel()
            {
                CatalogItems =  catalog.CatalogItems, 
                Brands = await _catalogService.GetBrands(),
                BrandFilterApplied = brandFilterApplied ??0,
                TypesFilterApplied = typeFilterApplied ??0,
                PaginationInfo = new PaginationInfo()
                {
                    CurrentPage = page?? 1,
                    PageSize = pageSize,
                    TotalCount = catalog.TotalCount,
                    TotalPages = (int)Math.Ceiling(((double)catalog.TotalCount /pageSize))
                }
            };
            catalogIndexViewModel.PaginationInfo.Next = (catalogIndexViewModel.PaginationInfo.CurrentPage == catalogIndexViewModel.PaginationInfo.TotalCount - 1) ? "is-disabled" : "";
            catalogIndexViewModel.PaginationInfo.Previous  = (catalogIndexViewModel.PaginationInfo.CurrentPage == 1)? "is-disabled" : "";
            return View(catalogIndexViewModel);
        }

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