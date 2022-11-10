using Fiorello.Areas.Admin.ViewModels.Expert;
using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExpertController(AppDbContext appDbContext,IFileService fileService,IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
           _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new ExpertIndexViewModel
            {
                Experts = await _appDbContext.Experts.ToListAsync()
            };
            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(ExpertCreateViewModel model)
        {
            if(!ModelState.IsValid) return View(model);

            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "The photo must be in image format");
                return View(model);
            }

            int maxSize = 500;

            if (!_fileService.CheckSize(model.Photo, maxSize))
            {
                ModelState.AddModelError("Photo", $"The photo size over than {maxSize} kb ");
                return View(model);
            }

            var expert = new Expert
            {
                Name = model.Name,
                Surname = model.Surname,
                Position = model.Position,
                PhotoName = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };
            await _appDbContext.Experts.AddAsync(expert);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Delete(int id)
        {
            var expert = await _appDbContext.Experts.FindAsync(id);
            if (expert == null) return NotFound();

            _fileService.Delete(expert.PhotoName, _webHostEnvironment.WebRootPath);

            _appDbContext.Experts.Remove(expert);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var expert = await _appDbContext.Experts.FindAsync(id);
            if (expert == null) return NotFound();

            var model = new ExpertDetailsViewModel
            {
                Id = expert.Id,
                Name = expert.Name,
                Surname = expert.Surname,
                Position = expert.Position,
                PhotoName = expert.PhotoName
            };
            return View(model);
        }


        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            var expert = await _appDbContext.Experts.FindAsync(id);

            if (expert == null) return NotFound();


            var model = new ExpertUpdateViewModel
            {
                Id = expert.Id,
                Name = expert.Name,
                Surname = expert.Surname,
                PhotoName = expert.PhotoName,
                Position = expert.Position
            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Update(ExpertUpdateViewModel model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var expert = await _appDbContext.Experts.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (expert == null) return NotFound();

            int maxSize = 500;

            if (model.Photo != null)
            {

                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", "The photo must be in image format");
                    return View(model);
                }
                if (!_fileService.CheckSize(model.Photo, maxSize))
                {
                    ModelState.AddModelError("Photo", $"The photo size over than {maxSize} kb ");
                    return View(model);
                }

                _fileService.Delete(expert.PhotoName, _webHostEnvironment.WebRootPath);
                expert.PhotoName = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            expert.Name = model.Name;
            expert.Position = model.Position;
            expert.Surname = model.Surname;
            model.PhotoName = expert.PhotoName;



            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
