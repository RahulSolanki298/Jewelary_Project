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

namespace ControlPanel.Controllers
{
    public class ProductStylesController : Controller
    {
        private readonly IProductStyleRepository _productStyles;
        private readonly IProductRepository _productRepository;
        public ProductStylesController(IProductStyleRepository productStyles,
            IProductRepository productRepository)
        {
            _productStyles = productStyles;
            _productRepository = productRepository;
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
            var result = await _productStyles.GetProductStyleItemsList();
            return PartialView("_ProductStyleItemList", result);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAllPoducts()
        {
            ViewBag.StyleList = await _productStyles.GetProductStyles();
            var productList = await _productRepository.GetProductStyleList();
            return PartialView("_JewelleryDataList", productList);
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
            if (styleId.HasValue && styleId.Value > 0)
            {
                var data = await _productStyles.GetProductStyleById(styleId.Value);
                if (data != null)
                {
                    var dt = new ProductStyleDTO();
                    dt.Id = data.Id;
                    dt.StyleName = data.StyleName;
                    dt.VenderId = data.VenderId;
                    dt.StyleImage = data.StyleImage;
                    dt.CoverPageImage = data.CoverPageImage;
                    dt.IsActivated = data.IsActivated;
                    dt.UpdatedDate = DateTime.Now;
                    return View(data);
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
