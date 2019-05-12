using System.Threading.Tasks;
using MicroServicesOnDocker.Web.WebMvc.Models;
using MicroServicesOnDocker.Web.WebMvc.Services;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;

namespace MicroServicesOnDocker.Web.WebMvc.ViewComponents
{
    public class CartList:ViewComponent
    {
        private readonly ICartService _cartService;

        public CartList(ICartService cartService) => _cartService = cartService;
        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user)
        {
            var vm = new Models.CartModels.Cart();
            try
            {
                vm = await _cartService.GetCart(user);

                
                return View(vm);
            }
            catch (BrokenCircuitException)
            {
                // Catch error when CartApi is in open circuit mode
                ViewBag.IsBasketInoperative = true;
                TempData["BasketInoperativeMsg"] = "Cart Service is unavailable, please try again later!. (Business Msg Due to Circuit-Breaker)";
            }
            return View(vm);
        }
    }
}
