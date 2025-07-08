using Business.Repository;
using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Common;

namespace ControlPanel.Controllers
{
    public class ProductStylesController : Controller
    {
        private readonly IProductStyleRepository _productStyles;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepositry _categoryRepository;
        public ProductStylesController(IProductStyleRepository productStyles,
            IProductRepository productRepository, ICategoryRepositry categoryRepositry)
        {
            _productStyles = productStyles;
            _productRepository = productRepository;
            _categoryRepository = categoryRepositry;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _productStyles.GetProductStyles();
            return View(result);
        }

        //GetProductStyleItemsList
        [HttpGet]
        public async Task<IActionResult> GetProductStyleItems()
        {
            var result = await _productStyles.GetProductStyleItemsList(SD.Activated);
            return PartialView("_ProductStyleItemList", result);
        }


        [HttpGet]
        public async Task<IActionResult> ShowAllProducts()
        {
            try
            {
                ViewBag.StyleList = await _productStyles.GetProductStyles();
                var productList = await _productRepository.GetProductStyleList();
                return PartialView("_JewelleryDataList", productList ?? new List<ProductMasterDTO>());
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateStatusRequest request)
        {
            if (request == null || request.Ids == null || !request.Ids.Any() || request.styleId == 0)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            try
            {
                var response = await _productStyles.AddOrUpdateBulkProductStyle(request);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log exception
                // _logger.LogError(ex, "Error updating product statuses");
                return Json(new { success = false, message = "An error occurred while updating." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> UpsertStyle(int? styleId = 0)
        {
            var result = new ProductStyleDTO();
            ViewBag.CategoryList = await _categoryRepository.GetCategoryList();
            if (styleId.HasValue && styleId.Value > 0)
            {
                var data = await _productStyles.GetProductStyleById(styleId.Value);
                if (data != null)
                {
                    result.Id = data.Id;
                    result.StyleName = data.StyleName;
                    result.VenderId = data.VenderId;
                    result.StyleImage = data.StyleImage;
                    result.CoverPageImage = data.CoverPageImage;
                    result.IsActivated = data.IsActivated;
                    result.UpdatedDate = DateTime.Now;
                    result.CategoryId = data.CategoryId;
                    return View(result);
                }
            }
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertStyle(ProductStyleDTO productStyles)
        {
            if (ModelState.IsValid)
            {
                if (productStyles.ImageFile != null && productStyles.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "styles");
                    Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(productStyles.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productStyles.ImageFile.CopyToAsync(stream);
                    }

                    // Save just the file name or relative path for DB
                    productStyles.StyleImage = Path.Combine("images", "styles", uniqueFileName);
                }

                if (productStyles.CoverPageFile != null && productStyles.CoverPageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "styles");
                    Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(productStyles.CoverPageFile.FileName);
                    var cvfilePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(cvfilePath, FileMode.Create))
                    {
                        await productStyles.CoverPageFile.CopyToAsync(stream);
                    }

                    // Save just the file name or relative path for DB
                    productStyles.CoverPageImage = Path.Combine("images", "styles", uniqueFileName);
                }

                var result = await _productStyles.SaveProductStyle(productStyles);
                if (result != true)
                {
                    TempData["Status"] = "Error";
                    TempData["Message"] = "Product Style has failed to save.";
                    return RedirectToAction("Index");
                }

                TempData["Status"] = "Success";
                TempData["Message"] = "Product Style saved successfully.";
                return RedirectToAction("Index");
            }

            return View(productStyles);
        }

    }
}
