using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroServicesOnDocker.Services.CartApi.Model;

namespace MicroServicesOnDocker.Services.CartApi.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class CartController : Controller
    {
        private ICartRepository _repository;
        private ILogger _logger;
        public CartController(ICartRepository repository, ILoggerFactory factory)
        {
            _repository = repository;
            _logger = factory.CreateLogger<CartController>();
        }
        // GET  api/v1/cart/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var cartAsync = await _repository.GetCartAsync(id);

            return Ok(cartAsync);
        }

        // POST api/v1/cart/
        [HttpPost]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]Cart cart)
        {
            var cartAsync = await _repository.UpdateCartAsync(cart);

            return Ok(cartAsync);
        }

        // DELETE  api/v1/cart/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _logger.LogInformation("Delete method in Cart controller reached");
            _repository.DeleteCartAsync(id);
        }
    }
}