using System.Collections.Generic;

namespace MicroServicesOnDocker.Services.CartApi.Model
{
    public class  Cart
    {
        public string CardId { get;  set; }
        public List<CartItem> Items { get; set; } 

        public Cart(string cartId)
        {
            CardId = cartId;
            Items = new List<Model.CartItem>();
        }
    }
}
