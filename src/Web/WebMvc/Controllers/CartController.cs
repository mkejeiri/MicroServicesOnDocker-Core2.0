using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroServicesOnDocker.Web.WebMvc.Models;
using MicroServicesOnDocker.Web.WebMvc.Models.CartModels;
using MicroServicesOnDocker.Web.WebMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
namespace MicroServicesOnDocker.Web.WebMvc.Controllers
{
    /*
     this acts as user front controller transmitting the request to cartService object to do CRUD ops on the basket/cart object
     */
    [Authorize]
    public class CartController : Controller
    {
        
        private readonly ICartService _cartService;
        private readonly ICatalogService _catalogService;
        private readonly IIdentityService<ApplicationUser> _identityService;

        public CartController(IIdentityService<ApplicationUser> identityService, ICartService cartService, ICatalogService catalogService)
        {
            _identityService = identityService;
            _cartService = cartService;
            _catalogService = catalogService;
        }
        public    IActionResult  Index()
        {
            //try
            //{

            //    var user = _identityService.Get(HttpContext.User);
            //    var cart = await _cartService.GetCart(user);


            //    return View();
            //}
            //catch (BrokenCircuitException)
            //{
            //    // Catch error when CartApi is in circuit-opened mode                 
            //    HandleBrokenCircuitException();
            //}

            //we get a list of cart item to display in the view here by using an embedded reusable razor web components 
            //which are derived from a specific class and can call service classes and return views after handing them some data.
            return View();
        }

        //here the page post on its self to update the data on the basket/cart.
        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> quantities, string action)
        {

            try
            {
                var user = _identityService.Get(HttpContext.User);
                var cart = await _cartService.SetQuantities(user, quantities);
                var vm = await _cartService.UpdateCart(cart);

                //if (action == "[ Checkout ]")
                //{
                //    var order = _cartService.MapBasketToOrder(basket);
                //    return RedirectToAction("Create", "Order");
                //}
            }
            //polly nuget package BrokenCircuitException class
            //to let know the user that the cartApi is not available and should try again later
            catch (BrokenCircuitException) 
            {
                // Catch error when CartApi is in open circuit  mode                 
                HandleBrokenCircuitException();
            }

            return View();

        }

            public async Task<IActionResult> AddToCart(CatalogItem productDetails)
        {
            try
            {
                if (productDetails.Id != null)
                {
                    var user = _identityService.Get(HttpContext.User);
                    var product = new CartItem()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Quantity = 1,
                        ProductName = productDetails.Name,
                        PictureUrl = productDetails.PictureUri,
                        UnitPrice = productDetails.Price,
                        ProductId = productDetails.Id
                    };
                    await _cartService.AddItemToCart(user, product);
                }
                return RedirectToAction("Index", "Catalog");
            }
            catch (BrokenCircuitException)
            {
                // Catch error when CartApi is in circuit-opened mode                 
                HandleBrokenCircuitException();
            }

            return RedirectToAction("Index", "Catalog");

        }
        //public async Task WriteOutIdentityInfo()
        //{
        //    var identityToken =
        //        await HttpContext.Authentication.
        //         GetAuthenticateInfoAsync(OpenIdConnectParameterNames.IdToken);
        //    Debug.WriteLine($"Identity Token: {identityToken}");
        //    foreach (var claim in User.Claims)
        //    {
        //        Debug.WriteLine($"Claim Type: {claim.Type} - Claim Value : {claim.Value}");
        //    }

        //}

        private void HandleBrokenCircuitException()
        {
            TempData["BasketInoperativeMsg"] = "cart Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)";
        }

    }
}