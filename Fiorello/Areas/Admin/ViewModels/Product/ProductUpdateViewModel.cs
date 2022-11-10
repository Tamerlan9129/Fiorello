﻿using Fiorello.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Fiorello.Models.Product;

namespace Fiorello.Areas.Admin.ViewModels.Product
{
    public class ProductUpdateViewModel
    {
        public ProductUpdateViewModel()
        {
            Photos = new List<IFormFile>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public string Weight { get; set; }
        public string Dimenesion { get; set; }


        public int CategoryId { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public ProductStatus Status { get; set; }

        public IFormFile? MainPhoto { get; set; }


        public string? MainPhotoName { get; set; }


        public List<IFormFile>? Photos { get; set; }

        public ICollection<Models.ProductPhoto>? ProductPhotos { get; set; }
    }
}
