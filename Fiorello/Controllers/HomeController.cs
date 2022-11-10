using Fiorello.DAL;
using Fiorello.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel
            {
                Experts = await _appDbContext.Experts.ToListAsync(),
                HomeMainSlider = await _appDbContext.HomeMainSlider.Include(ms => ms.HomeMainSliderPhotos.OrderBy(ms=>ms.Order)).FirstOrDefaultAsync()
            };
            return View(model);
        }

    }
}
