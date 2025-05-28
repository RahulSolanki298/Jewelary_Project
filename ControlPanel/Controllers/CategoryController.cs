using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepositry _categoryRepository;

        public CategoryController(ICategoryRepositry categoryRepository)
        {
            _categoryRepository = categoryRepository;
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

        [HttpGet]
        public async Task<IActionResult> SaveCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.SaveCategory(category);
                TempData["Status"] = "Success";
                TempData["Message"] = "Product category has been updated successfully.";
                return RedirectToAction("Index");
            }

            return View(category);
        }
    }
}
