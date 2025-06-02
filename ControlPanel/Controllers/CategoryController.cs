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
        public async Task<IActionResult> StyleAndCollectionList(int categoryId,string type)
        {
            if (type == "styles")
            {
                var data = await _productStyleRepository.GetProductStyleByCategoryId(categoryId);
                return PartialView("_GetProductStyleList", data);
            }
            else if (type == "collections")
            {
                var data = await _productStyleRepository.GetProductCollectionsByCategoryId(categoryId);
                return PartialView("_GetProductCollectionList", data);
            }
            return null;   
        }


        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id = 0)
        {
            if (id > 0)
            {
                var result = await _categoryRepository.DeleteCategoryById(id);

                if (result==true)
                {
                    TempData["Status"] = "Success";
                    TempData["Message"] = "Product category has been deleted successfully.";
                }
                else
                {
                    TempData["Status"] = "Fail";
                    TempData["Message"] = "Product category has been deleted successfully.";
                }

                return RedirectToAction("Index");
            }

            return View();
        }

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
