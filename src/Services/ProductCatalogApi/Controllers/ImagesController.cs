using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MicroServicesOnDocker.Services.ProductCatalogApi.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IHostingEnvironment _environment;
        public ImagesController(IHostingEnvironment environment )
        {
            _environment = environment;
        }

        // GET: api/Images/5
        [HttpGet()]
        [Route("{id}")]
        public IActionResult GetImage(int id)
        {
            var wwwroot = _environment.WebRootPath;
            var imageFullPath = Path.Combine($"{wwwroot}","Images", $"shoes-{id}.png");
            var buffer = System.IO.File.ReadAllBytes(imageFullPath);
            return File(buffer,"Image/png");
        }
    } 
}