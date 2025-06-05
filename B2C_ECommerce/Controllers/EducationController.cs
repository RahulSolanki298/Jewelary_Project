using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2C_ECommerce.Controllers
{
    public class EducationController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        public EducationController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _blogRepository.GetBlogCategoryList();
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> BlogCategory()
        {
            var response = await _blogRepository.GetBlogList();
            return PartialView("_BlogDataList",response);
        }
    }
}
