using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MicroServicesOnDocker.Web.WebMvc.Infrastructure;
using MicroServicesOnDocker.Web.WebMvc.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace MicroServicesOnDocker.Web.WebMvc.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IOptionsSnapshot<AppSettings> _setting;
        private readonly IHttpClient _httpApiClient;
        private readonly ILogger<CatalogService> _logger;
        private readonly string _remoteServiceBaseUrl;

        public CatalogService(IOptionsSnapshot<AppSettings> setting, IHttpClient httpApiClient, ILogger<CatalogService> logger)
        {
            _setting = setting;
            _httpApiClient = httpApiClient;
            _logger = logger;
            _remoteServiceBaseUrl = $"{_setting.Value.CatalogUrl}/api/catalog";
        }

        public async Task<Catalog> GetCatalogItem(int page, int take, int? brand, int? type)
        {
            var catalogItemsUri = ApiPaths.Catalog.GetAllCatalogItems(baseUri: _remoteServiceBaseUrl, page: page,
                take: take, brand: brand, type: type);

            var catalogItemDataString = await _httpApiClient.GetStringAsync(uri: catalogItemsUri);
            var response = JsonConvert.DeserializeObject<List<CatalogItem>>(catalogItemDataString);
            return new Catalog()
            {
                CurrentPage = page,
                PageSize = take,
                CatalogItems = response,
                TotalCount = response.Count(),
                TotalPages = response.Count() * take
            };
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var brandsUri = ApiPaths.Catalog.GetAllBrands(baseUri: _remoteServiceBaseUrl);
            var brandsDataString = await _httpApiClient.GetStringAsync(brandsUri);
            var brands = JArray.Parse(brandsDataString);
            var items = new List<SelectListItem>()
            {
                new SelectListItem {
                Value = null,
                Text = "All"
            }};
            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("brand")
                });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var typesUri = ApiPaths.Catalog.GetAllTypes(baseUri: _remoteServiceBaseUrl);
            var typesDataString = await _httpApiClient.GetStringAsync(typesUri);
            var types = JArray.Parse(typesDataString);
            var items = new List<SelectListItem>()
                {
                    new SelectListItem {
                    Value = null,
                    Text = "All"
                }};
            foreach (var type in types.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = type.Value<string>("id"),
                    Text = type.Value<string>("type")
                });
            }
            return items;
        }
    }
}