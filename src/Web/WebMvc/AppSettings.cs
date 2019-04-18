
namespace MicroServicesOnDocker.Web.WebMvc
{
    public class AppSettings
    {
        public string CatalogUrl { get; set; }
        public Logging Logging { get; set; }

       
    }
    public class Logging
    {
        public bool IncludeScope { get; set; }
        public Microsoft.Extensions.Logging.LogLevel LogLevel { get; set; }
    }
}
