using Fiorello.DAL;
using Fiorello.Models;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new ProductIndexViewModel
            {
                Products = await _appDbContext.Products.OrderByDescending(p => p.Id)
                                                               .Take(4).
                                                               ToListAsync()
            };
            return View(model);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _appDbContext.Products
                                 .Include(p => p.ProductPhotos)
                                 .Include(p => p.Category)
                                 .FirstOrDefaultAsync(p => p.Id == id);
            if(product == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Status = product.Status,
                Category = product.Category,
                Description = product.Description,
                Quantity = product.Quantity,
                Title = product.Title,
                Dimenesion = product.Dimenesion,
                MainPhoto = product.MainPhotoName,
                Weight = product.Weight,
                Price = product.Price,
                Photos = product.ProductPhotos
            };
            return View(model);
        }

        public async Task<IActionResult> LoadMore(int skipRow)
        {
            var product= await _appDbContext.Products.OrderByDescending(p=>p.Id)
                                                   .Skip(4*skipRow)
                                                   .Take(4)
                                                   .ToListAsync();
            bool isLast = false;
            if (((skipRow + 1) * 3 + 1) > _appDbContext.Products.Count())
            {
                isLast = true;
            }

            if (product.Count < 4)
            {
                isLast = true;
            }
            var model = new ProductLoadMoreViewModel
            {
                Products = product,
                IsLast = isLast
            };
            return PartialView("_ProductPartial",model);
        }
    }
}
