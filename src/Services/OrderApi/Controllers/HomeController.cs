using Microsoft.AspNetCore.Mvc;

namespace MicroServicesOnDocker.Services.OrderApi.Controllers
{
    public class HomeController : Controller
    {

        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}