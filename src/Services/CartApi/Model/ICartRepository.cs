using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServicesOnDocker.Services.CartApi.Model
{
    public interface ICartRepository
    {
        Task<Cart> GetCartAsync(string cardId);
         IEnumerable<string>  GetUsers();
        Task<Cart> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(string cardId);
    }
}
