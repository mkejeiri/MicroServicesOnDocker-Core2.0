using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicesOnDocker.Web.WebMvc.Infrastructure;
using MicroServicesOnDocker.Web.WebMvc.Models;
using MicroServicesOnDocker.Web.WebMvc.Models.CartModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MicroServicesOnDocker.Web.WebMvc.Services
{
    public class CartService : ICartService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private IHttpContextAccessor _httpContextAccesor;
        private readonly ILogger _logger;
        public CartService(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor, IHttpClient httpClient, ILoggerFactory logger)
        {
            _settings = settings;
            _remoteServiceBaseUrl = $"{_settings.Value.CartUrl}/api/v1/cart";
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _logger = logger.CreateLogger<CartService>();
        }


        public async Task AddItemToCart(ApplicationUser user, CartItem product)
        {
            var cart = await GetCart(user);
            _logger.LogDebug("User Name: " + user.Id);
            if (cart == null)
            {
                cart = new Cart()
                {
                    BuyerId = user.Id,
                    Items = new List<CartItem>()
                };
            }
            var cartItem = cart.Items
                .FirstOrDefault(p => p.ProductId == product.ProductId);
            if (cartItem == null)
            {
                cart.Items.Add(product);
            }
            else
            {
                cartItem.Quantity += 1;
            }


            await UpdateCart(cart);
        }

        public async Task ClearCart(ApplicationUser user)
        {
            var token = await GetUserTokenAsync();
            var cleanCartUri = ApiPaths.Cart.CleanCart(_remoteServiceBaseUrl, user.Id);
            _logger.LogDebug("Clean Cart uri : " + cleanCartUri);
            var response = await _apiClient.DeleteAsync(cleanCartUri);
            _logger.LogDebug("Cart cleaned");
        }

        public async Task<Cart> GetCart(ApplicationUser user)
        {
            var token = await GetUserTokenAsync();
          //  string parse = JArray.Parse(token).ToString();
            _logger.LogInformation(" We are in get cart and user id " + user.Id);
            _logger.LogInformation(_remoteServiceBaseUrl);

            var getCarttUri = ApiPaths.Cart.GetCart(_remoteServiceBaseUrl, user.Id);
            _logger.LogInformation(getCarttUri);
            var dataString = await _apiClient.GetStringAsync(getCarttUri, token);
            _logger.LogInformation(dataString);
            // Use the ?? Null conditional operator to simplify the initialization of response
            //var response = JsonConvert.DeserializeObject<Cart>(dataString) ??
            //    new Cart()
            //    {
            //        BuyerId = user.Id
            //    };

            var response = JsonConvert.DeserializeObject<Cart>(dataString.ToString()) ??
               new Cart()
               {
                   BuyerId = user.Id
               };
            return response;
        }

        

        public async Task<Cart> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities)
        {
            var cart = await GetCart(user);

            cart.Items.ForEach(x =>
            {
                // Simplify this logic by using the
                // new out variable initializer.
                if (quantities.TryGetValue(x.Id, out var quantity))
                {
                    x.Quantity = quantity;
                }
            });

            return cart;
        }

        public async Task<Cart> UpdateCart(Cart cart)
        {

              var token = await GetUserTokenAsync();
            _logger.LogDebug("Service url: " + _remoteServiceBaseUrl);
            var updateCartUri = ApiPaths.Cart.UpdateCart(_remoteServiceBaseUrl);
            _logger.LogDebug("Update Cart url: " + updateCartUri);
            var response = await _apiClient.PostAsync(updateCartUri, cart,token); 
            response.EnsureSuccessStatusCode();

            return cart;
        }

        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.GetTokenAsync("access_token");
          
        }
    }
}
