using Fiorello.Areas.Admin.ViewModels.Category;
using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public CategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task <IActionResult> Index()
        {
            var model = new CategoryIndexViewModel
            {
                Categories = await _appDbContext.Categories.ToListAsync()
            };
            return View(model);
        }
        #region Create
        [HttpGet]

        public async Task<IActionResult> Create()
        {

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if(!ModelState.IsValid) return View(model);
            
            bool isExist = await _appDbContext.Categories.
                                   AnyAsync(c=>c.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "Bu adda category movcuddur");
                return View(model);
            }
            var category = new Category
            {
                Id = model.Id,
                Title = model.Title
            };
            await _appDbContext.Categories.AddAsync(category);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }
        #endregion
        #region Update
        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            var category = await _appDbContext.Categories.FindAsync(id);

            if (category == null) return NotFound();

            var model = new CategoryUpdateViewModel
            {
                Title = category.Title,
                Id = category.Id
            };

            return View(model);

            
        }

        [HttpPost]

        public async Task <IActionResult> Update(int id,CategoryUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (id != model.Id) return BadRequest();

            var dbcategory = await _appDbContext.Categories.FindAsync(id);
            if (dbcategory == null) return NotFound();
            bool isExist = await _appDbContext.Categories.
                                   AnyAsync(c => c.Title.ToLower().Trim() == model.Title.ToLower().Trim()&& c.Id!=model.Id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "Bu adda category movcuddur");
                return View(model);
            }

            dbcategory.Title = model.Title;

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");

        }
        #endregion
        #region Delete
        [HttpGet]

        

        public async Task<IActionResult> Delete(int id)
        {
            var dbcategory = await _appDbContext.Categories.FindAsync(id);

            if(dbcategory==null) return NotFound();

            _appDbContext.Categories.Remove(dbcategory);

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");

        }
        #endregion

        [HttpGet]

        public async Task<IActionResult> Details(int id)
        {
            var category = await _appDbContext.Categories.FindAsync(id);

            if (category == null) return NotFound();
            var model = new CategoryDetailsViewModel
            {
                Title = category.Title,
                Id = category.Id
            };

            return View(model);
        }
    }
}
