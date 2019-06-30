namespace MicroServicesOnDocker.Web.WebMvc.Models.OrderModels
{
    public class OrderItem
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }


        public int Units { get; set; }

        public string PictureUrl { get; set; }

    }
}
