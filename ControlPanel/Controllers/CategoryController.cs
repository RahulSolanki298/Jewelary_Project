using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepositry _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductStyleRepository _productStyleRepository;

        public CategoryController(ICategoryRepositry categoryRepository,
            IProductRepository productRepository,
            IProductStyleRepository productStyleRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _productStyleRepository = productStyleRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CategoryList()
        {
            var data = await _categoryRepository.GetCategoryList();
            return PartialView("_CategoryList", data);
        }

        [HttpGet]
        public async Task<IActionResult> StyleList(int categoryId)
        {
            var data = await _productStyleRepository.GetProductStyleByCategoryId(categoryId);
            return PartialView("_GetProductStyleList", data);
        }

        [HttpGet]
        public async Task<IActionResult> CollectionList(int categoryId)
        {
            var data = await _categoryRepository.GetCategoryList();
            return PartialView("_GetProductCollectionList", data);
        }


        //[HttpGet]
        //public async Task<IActionResult> CategoryList()
        //{
        //    bool status = false;
        //    string strResult = "";
        //    string strMessage = "Data Not Found";

        //    var data = await _categoryRepository.GetCategoryList();

        //    if (data != null)
        //    {
        //        status = true;
        //        strMessage = "";
        //        strResult = JsonConvert.SerializeObject(data);
        //    }
        //    return Json(new
        //    {
        //        Data = new
        //        {
        //            status = status,
        //            result = strResult,
        //            message = strMessage
        //        }
        //    });
        //}

        [HttpGet]
        public async Task<IActionResult> GetCategory(int? id = 0)
        {
            if (id.Value > 0)
            {
                var result = await _categoryRepository.GetCategoryById(id.Value);
                return View(result);
            }
            
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> SaveCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                // Save uploaded image
                if (category.ImageFile != null && category.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploadsFolder); // Safe if it already exists

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(category.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await category.ImageFile.CopyToAsync(stream);
                    }

                    category.CategoryImage = "/uploads/" + uniqueFileName;
                }

                await _categoryRepository.SaveCategory(category);

                TempData["Status"] = "Success";
                TempData["Message"] = "Product category has been saved successfully.";
                return RedirectToAction("Index");
            }

            return View(category);
        }


    }
}
