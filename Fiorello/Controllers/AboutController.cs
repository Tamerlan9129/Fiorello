using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
