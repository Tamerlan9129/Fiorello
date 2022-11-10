using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class FaqPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
