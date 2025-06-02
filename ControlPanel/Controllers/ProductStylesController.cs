using Business.Repository;
using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq;
using System;
using System.Threading.Tasks;

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


        [HttpPost]
        public async Task<IActionResult> UpsertStyle(ProductStyleDTO productStyles)
        {
            if (ModelState.IsValid)
            {
              var result= await _productStyles.SaveProductStyle(productStyles);
                if (result != true)
                {
                    TempData["Status"] = "Error";
                    TempData["Message"] = "Product Style has been failed";
                    return RedirectToAction("Index");
                }
                TempData["Status"] = "Success";
                TempData["Message"] = "Product Style Save Sucessfully..";
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
