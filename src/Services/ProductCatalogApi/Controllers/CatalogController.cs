using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductCatalogApi.Domain.Entities;
using ProductCatalogApi.Domain.Repository;
namespace ProductCatalogApi.Controllers
{
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public CatalogController(ICatalogRepository catalogRepository, IOptionsSnapshot<AppSettings> settings)
        {
            _catalogRepository = catalogRepository;
            _settings = settings;
            //string externalBaseUrl = _settings.Value.ExternalBaseUrl;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Types() => Ok(await _catalogRepository.GetCatalogTypesAsync());

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items() => Ok(await _catalogRepository.GetCatalogItemsAsync());

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Brands() => Ok(await _catalogRepository.GetCatalogBrandsAsync());


        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if (id > 0)
            {
                var item = await _catalogRepository.GetItemAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                string externalBaseUrl = _settings.Value.ExternalBaseUrl.Split(';')[0];
                item.PictureUri = item.PictureUri.Replace("[externalBaseUrl]", externalBaseUrl);
                //use a DTO mapper here
                return Ok(item);
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetItems([FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            string externalBaseUrl = _settings.Value.ExternalBaseUrl.Split(';')[0];
            var itemsDto = await _catalogRepository.GetPagedCatalogItemsAsync(pageIndex, pageSize);
            //use a DTO mapper here
            return Ok(itemsDto.Select(x =>
            {
                x.PictureUri = x.PictureUri.Replace("[externalBaseUrl]", externalBaseUrl);
                return x;
            }));
        }

        [HttpGet]
        [Route("[action]/like/{name:minlength(1)}")]
        public async Task<IActionResult> ItemsLikeName(string name, [FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            string externalBaseUrl = _settings.Value.ExternalBaseUrl.Split(';')[0];
            var itemsDto = await _catalogRepository.GetPagedCatalogItemsByNameAsync(name, pageIndex, pageSize);
            //use a DTO mapper here
            return Ok(itemsDto.Select(x =>
            {
                x.PictureUri = x.PictureUri.Replace("[externalBaseUrl]", externalBaseUrl);
                return x;
            }));
        }
        //GET api/catalog/items/type/1/brand/null?pagesize=4&pageindex=0
        //GET api/catalog/items/type/1/brand/2?pagesize=4&pageindex=0
        [HttpGet]
        [Route("[action]/type/{typeId}/brand/{brandId}")]
        public async Task<IActionResult> ItemsTypeBrand(int? typeId, int? brandId, [FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 1)
        {
            if (pageIndex<1)
            {
                return BadRequest("Incorrect page index error");
            }

            string externalBaseUrl = _settings.Value.ExternalBaseUrl.Split(';')[0];
            var itemsDto = await _catalogRepository.GetPagedCatalogItemsByTypeBrandAsync(typeId, brandId, pageIndex, pageSize);
           
            //use a DTO mapper here
            return Ok(itemsDto.Select(x =>
            {
                x.PictureUri = x.PictureUri.Replace("[externalBaseUrl]", externalBaseUrl);
                return x;
            }));
        }

        [HttpPost]
        [Route("items")]
        public async Task<IActionResult> CreateProduct([FromBody] CatalogItem product)
        {
           int id = await _catalogRepository.AddAsync(new CatalogItem
            {
                CatalogBrandId = product.CatalogBrandId,
                CatalogTypeId = product.CatalogTypeId,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                PictureFileName = product.PictureFileName
            });
           return CreatedAtAction(nameof(GetItemById), new {id});
        }

        [HttpPut]
        [Route("items")]
        public async Task<IActionResult> UpdateProduct([FromBody] CatalogItem productToUpdate)
        {
            if (!await _catalogRepository.Exists(productToUpdate.Id) )
            {
                return NotFound($"Product with id  {productToUpdate.Id} is not found");
            }

            await _catalogRepository.UpdateCatalogItem(productToUpdate);
            return CreatedAtAction(nameof(GetItemById), new { id = productToUpdate.Id });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!await _catalogRepository.Exists(id))
            {
                return NotFound($"Product with id  {id} is not found");
            }

            await _catalogRepository.DeleteCatalogItem(id);
            return NoContent();
        }
    }
}