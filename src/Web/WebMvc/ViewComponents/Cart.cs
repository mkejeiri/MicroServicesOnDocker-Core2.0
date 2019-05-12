using System.Threading.Tasks;
using MicroServicesOnDocker.Web.WebMvc.Models;
using MicroServicesOnDocker.Web.WebMvc.Services;
using MicroServicesOnDocker.Web.WebMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;

namespace MicroServicesOnDocker.Web.WebMvc.ViewComponents
{
    public class Cart:ViewComponent
    {
        private readonly ICartService _cartService;

        public Cart(ICartService cartService) => _cartService = cartService;
        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user) 
        {

            
            var vm = new CartComponentViewModel();
            try
            {
                var cart = await _cartService.GetCart(user);

                vm.ItemsInCart = cart.Items.Count;
                vm.TotalCost = cart.Total();
                return View(vm);
            }
            catch(BrokenCircuitException)
            {
                // Catch error when CartApi is in open circuit mode
                ViewBag.IsBasketInoperative = true;
            }
            
            return View(vm);
        }

    }
}
