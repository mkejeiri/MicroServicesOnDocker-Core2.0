using Microsoft.AspNetCore.Mvc;

namespace MicroServicesOnDocker.Services.CartApi.Controllers
{
    public class HomeController : Controller
    {

        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger/ui");
        }
    }
}