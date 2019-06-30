using System.Collections.Generic;
using System.Threading.Tasks;
using MicroServicesOnDocker.Web.WebMvc.Infrastructure;
using MicroServicesOnDocker.Web.WebMvc.Models;
using MicroServicesOnDocker.Web.WebMvc.Models.CartModels;
using MicroServicesOnDocker.Web.WebMvc.Models.OrderModels;

namespace MicroServicesOnDocker.Web.WebMvc.Services
{
    //this will send request to cartApi to fetch a user cart/basket
    public interface ICartService
    {
        Task<Cart> GetCart(ApplicationUser user);
        Task AddItemToCart(ApplicationUser user, CartItem product);
        Task<Cart> UpdateCart(Cart Cart);
        Task<Cart> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities);
        Order MapCartToOrder(Cart Cart);
        Task ClearCart(ApplicationUser user);

    }
}
