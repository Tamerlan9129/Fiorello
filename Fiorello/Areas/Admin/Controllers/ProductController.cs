using Fiorello.Areas.Admin.ViewModels.Product;
using Fiorello.Areas.Admin.ViewModels.Product.ProductPhoto;
using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new ProductIndexViewModel
            {
                Products = await _appDbContext.Products.ToListAsync()
            };
            return View(model);
        }
        #region Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProductCreateViewModel
            {
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();
            if (!ModelState.IsValid) return View(model);
            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "This category does  not exist");
                return View(model);
            }
            bool isExist = await _appDbContext.Products.AnyAsync(p => p.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "This title is already exist");
                return View(model);
            }
            if (!_fileService.IsImage(model.MainPhoto))
            {
                ModelState.AddModelError("MainPhoto", "The photo must be in image format");
                return View(model);
            }
            int maxSize = 500;
            if (!_fileService.CheckSize(model.MainPhoto, maxSize))
            {
                ModelState.AddModelError("MainPhoto", $"The photo size over than {maxSize} kb ");
                return View(model);
            }

            bool hasError = false;
            foreach (var photo in model.Photos)
            {
                if (!_fileService.IsImage(photo))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName}  photo must be in image format");
                    hasError = true;

                }
                else if (!_fileService.CheckSize(photo, maxSize))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName}  photo size over than {maxSize} kb");
                    hasError = true;

                }

            }

            if (hasError) return View(model);

            var product = new Product
            {
                Title = model.Title,
                Price = model.Price,
                Description = model.Description,
                Quantity = model.Quantity,
                Weight = model.Weight,
                Dimenesion = model.Dimenesion,
                CategoryId = model.CategoryId,
                Status = model.Status,
                MainPhotoName = await _fileService.UploadAsync(model.MainPhoto, _webHostEnvironment.WebRootPath)
            };
            await _appDbContext.Products.AddAsync(product);
            await _appDbContext.SaveChangesAsync();
            int order = 1;
            foreach (var photo in model.Photos)
            {
                var productPhoto = new ProductPhoto
                {
                    ProductId = product.Id,
                    Order = order,
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath)
                };
                await _appDbContext.ProductPhotos.AddAsync(productPhoto);
                await _appDbContext.SaveChangesAsync();

                order++;
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _appDbContext.Products.
                           Include(p => p.ProductPhotos).
                       FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var model = new ProductUpdateViewModel
            {

                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                Quantity = product.Quantity,
                Weight = product.Weight,
                Dimenesion = product.Dimenesion,
                CategoryId = product.CategoryId,
                Status = product.Status,
                MainPhotoName = product.MainPhotoName,
                ProductPhotos = product.ProductPhotos,



                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()

            };

            return View(model);


        }
        [HttpPost]

        public async Task<IActionResult> Update(int id,ProductUpdateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();
            if (!ModelState.IsValid) return View(model);
            if(id!=model.Id) return BadRequest();

            var product = await _appDbContext.Products.
                               Include(p => p.ProductPhotos).
                                FirstOrDefaultAsync(p => p.Id == id);

            model.ProductPhotos = product.ProductPhotos.ToList();

            if (product == null) return NotFound();

            bool isExits = await _appDbContext.Products.AnyAsync(p => p.Title.ToLower().Trim() == product.Title.ToLower().Trim() && p.Id != product.Id);

            if (isExits)
            {
                ModelState.AddModelError("Title", "This title is already exist");
                return View(model);
            }

            product.Title = model.Title;
            product.Weight = model.Weight;
            product.Price = model.Price;
            product.Description = model.Description;
            product.Dimenesion = model.Dimenesion;
            product.Quantity = model.Quantity;
            product.Status = model.Status;
          

            int maxSize = 500;
            if (model.MainPhoto != null)
            {
                if (!_fileService.IsImage(model.MainPhoto))
                {
                    ModelState.AddModelError("Photo", "The photo must be in image format");
                    return View(model);
                }
                if (!_fileService.CheckSize(model.MainPhoto, maxSize))
                {
                    ModelState.AddModelError("Photo", $"The photo size over than {maxSize} kb ");
                    return View(model);
                }

                _fileService.Delete(product.MainPhotoName, _webHostEnvironment.WebRootPath);
                product.MainPhotoName = await _fileService.UploadAsync(model.MainPhoto, _webHostEnvironment.WebRootPath);
            }

            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);
            if (category == null) return NotFound();
            product.CategoryId = category.Id;


            await _appDbContext.SaveChangesAsync();


            bool hasError = false;
            foreach (var photo in model.Photos)
            {
                if (!_fileService.IsImage(photo))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName}  photo must be in image format");
                    hasError = true;

                }
                else if (!_fileService.CheckSize(photo, maxSize))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName}  photo size over than {maxSize} kb");
                    hasError = true;

                }

            }

            if (hasError) return View(model);

            int order = 1;
            foreach (var photo in model.Photos)
            {
                var productPhoto = new ProductPhoto
                {
                    ProductId = product.Id,
                    Order = order,
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath)
                    
                };
                await _appDbContext.ProductPhotos.AddAsync(productPhoto);
                await _appDbContext.SaveChangesAsync();

                order++;
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _appDbContext.Products.
                Include(p => p.ProductPhotos).
                FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            _fileService.Delete(product.MainPhotoName, _webHostEnvironment.WebRootPath);

            foreach (var photo in product.ProductPhotos)
            {
                _fileService.Delete(photo.Name, _webHostEnvironment.WebRootPath);

            }
            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(int id)
        {
            var product = await _appDbContext.Products.
                                 Include(p => p.ProductPhotos).
                                FirstOrDefaultAsync(p => p.Id == id);



            if (product == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                Quantity = product.Quantity,
                Weight = product.Weight,
                Dimenesion = product.Dimenesion,
                CategoryId = product.CategoryId,
                Status = product.Status,
                MainPhoto = product.MainPhotoName,
                Photos = product.ProductPhotos,
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()

            };

            return View(model);
        }
        #endregion

        #region ProductPhotoUpdate
        [HttpGet]


        public async Task<IActionResult> UpdatePhoto(int id)
        {

            var productPhoto = await _appDbContext.ProductPhotos.FindAsync(id);
            if (productPhoto == null) return NotFound();

            var model = new ProductPhotoUpdateViewModel
            {
                Order = productPhoto.Order
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, ProductPhotoUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (id != model.Id) return BadRequest();

            var productPhoto = await _appDbContext.ProductPhotos.FindAsync(model.Id);
            if (productPhoto == null) return NotFound();

            productPhoto.Order = model.Order;
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("update", "product", new { id = productPhoto.ProductId });


        }
        #endregion

        #region ProductPhotoDelete
        [HttpGet]

        public async Task<IActionResult> Deletephoto(int id)
        {
            var productPhoto = await _appDbContext.ProductPhotos.FindAsync(id);
            if (productPhoto == null) return NotFound();


            _fileService.Delete(productPhoto.Name, _webHostEnvironment.WebRootPath);

            _appDbContext.ProductPhotos.Remove(productPhoto);
            await _appDbContext.SaveChangesAsync();


            return RedirectToAction("update", "product", new { id = productPhoto.ProductId });
        }
        #endregion
    }
}
